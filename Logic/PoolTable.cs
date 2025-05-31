using Data;
using System; // Potrzebne dla Random
using System.Collections.Generic;
using System.Numerics; // Potrzebne dla Vector2
using System.Threading; // Potrzebne dla Thread.Sleep
using System.Threading.Tasks; // Potrzebne dla Task

namespace Logic
{
    internal class PoolTable : LogicAbstractAPI
    {
        private int _width;
        private int _height;
        private List<Ball> _balls = new List<Ball>();
        private DataAbstractAPI _data; // Pole _data jest zdefiniowane, ale nie używane w tym kodzie

        private bool _isRunning = false; // Flaga kontrolująca pętlę symulacji
        private Task? _simulationTask; // Zadanie uruchamiające symulację w tle

        public PoolTable(int width, int height, DataAbstractAPI data)
        {
            _width = width;
            _height = height;
            _data = data; // Przypisanie _data
        }

        public override void CreateBalls(int ballsQuantity, int radius)
        {
            Random random = new Random();
            _balls.Clear(); // Wyczyść poprzednie piłki, jeśli były

            // Minimalna odległość między środkami piłek przy tworzeniu
            float minDistanceSquared = (2f * radius) * (2f * radius) * 1.1f; // Dodajemy mały bufor 1.1f

            for (int i = 0; i < ballsQuantity; i++)
            {
                float x, y;
                bool positionFound = false;
                int attempts = 0;
                int maxAttempts = 100; // Ogranicz liczbę prób znalezienia wolnego miejsca

                // Pętla próbująca znaleźć wolne miejsce dla nowej piłki
                while (!positionFound && attempts < maxAttempts)
                {
                    // Generuj losową pozycję w granicach planszy, uwzględniając promień piłki
                    x = random.Next(radius, _width - radius);
                    y = random.Next(radius, _height - radius);

                    // Sprawdź, czy nowa pozycja nie koliduje z istniejącymi piłkami
                    bool colliding = false;
                    foreach (Ball existingBall in _balls)
                    {
                        float distanceSquared = Vector2.DistanceSquared(new Vector2(x, y), new Vector2(existingBall.X, existingBall.Y));
                        if (distanceSquared < minDistanceSquared)
                        {
                            colliding = true;
                            break; // Znaleziono kolizję, spróbuj ponownie
                        }
                    }

                    if (!colliding)
                    {
                        // Znaleziono wolne miejsce
                        // Dodajemy losową prędkość początkową
                        // Generuj prędkość w zakresie od -2 do 2
                        float vx = (float)(random.NextDouble() * 4 - 2);
                        float vy = (float)(random.NextDouble() * 4 - 2);

                        Ball ball = new Ball(x, y, radius, vx, vy);
                        _balls.Add(ball);
                        positionFound = true; // Zakończ szukanie pozycji dla tej piłki
                    }
                    attempts++;
                }

                // Opcjonalnie: obsłuż przypadek, gdy nie udało się znaleźć miejsca po wielu próbach
                if (!positionFound)
                {
                    // Możesz zalogować ostrzeżenie lub rzucić wyjątek,
                    // albo po prostu stworzyć mniej piłek niż założono.
                    // Na razie po prostu kontynuujemy, co może skutkować mniejszą liczbą piłek.
                    System.Diagnostics.Debug.WriteLine($"Warning: Could not find a non-colliding position for ball {i + 1}");
                }
            }
        }

        public override void StartGame()
        {
            // Uruchamiamy główną pętlę symulacji tylko raz, jeśli nie jest już uruchomiona
            if (!_isRunning)
            {
                _isRunning = true;
                // Uruchamiamy główną pętlę symulacji w tle przy użyciu Task.Run
                _simulationTask = Task.Run(() => RunSimulation());
            }
        }

        public override void StopGame()
        {
            _isRunning = false; // Ustawiamy flagę na false, aby zakończyć pętlę symulacji

            // Czekamy na zakończenie zadania symulacji
            // Używamy ?.Wait(), aby uniknąć NullReferenceException, jeśli Task nigdy nie został uruchomiony
            _simulationTask?.Wait();

            _balls.Clear(); // Wyczyść listę piłek
        }

        public override int Width
        {
            get { return _width; }
        }

        public override int Height
        {
            get { return _height; }
        }

        public override List<Ball> GetAllBalls()
        {
            // Zwracamy listę piłek. W bardziej złożonych aplikacjach,
            // jeśli lista jest modyfikowana z różnych wątków,
            // warto rozważyć zwracanie kopii lub użycie blokady.
            return _balls;
        }

        // Główna pętla symulacji działająca w tle
        private void RunSimulation()
        {
            // Czas opóźnienia dla kontroli prędkości symulacji (w milisekundach)
            // Około 16 ms to 60 klatek na sekundę.
            int simulationDelay = 16;

            while (_isRunning) // Pętla działa dopóki _isRunning jest true
            {
                // Używamy blokady, aby bezpiecznie iterować po liście piłek
                // i modyfikować ich stan. Zapobiega to problemom z konkurencyjnym dostępem.
                lock (_balls)
                {
                    // 1. Zaktualizuj pozycje piłek i sprawdź kolizje ze ścianami
                    // Metoda Move w Ball.cs aktualizuje pozycję na podstawie Vx/Vy
                    // i odwraca Vx/Vy przy kolizji ze ścianą.
                    foreach (Ball ball in _balls)
                    {
                        ball.Move(_width, _height);
                    }

                    // 2. Sprawdź i rozwiąż kolizje między piłkami
                    CheckBallCollisions();
                }

                // Opcjonalnie: Powiadom View/ViewModel o globalnej aktualizacji
                // (jeśli potrzebne jest zdarzenie na poziomie PoolTable)

                // Opuść wątek na krótki czas, aby kontrolować tempo symulacji
                Thread.Sleep(simulationDelay);
            }
        }

        // Metoda sprawdzająca kolizje między wszystkimi parami piłek
        private void CheckBallCollisions()
        {
            float collisionFactor = 0.75f; 
    
            for (int i = 0; i < _balls.Count; i++)
            {
                for (int j = i + 1; j < _balls.Count; j++)
                {
                    Ball ball1 = _balls[i];
                    Ball ball2 = _balls[j];

                    // Oblicz odległość między środkami piłek
                    float distance = Vector2.Distance(new Vector2(ball1.X, ball1.Y), new Vector2(ball2.X, ball2.Y));

                    // Zmodyfikowany warunek kolizji z niższym współczynnikiem korekcji
                    float collisionThreshold = (ball1.Radius + ball2.Radius) * collisionFactor;

                    if (distance <= collisionThreshold)
                    {
                        // Wykryto kolizję, rozwiąż ją
                        ResolveBallCollision(ball1, ball2);
                    }
                }
            }
        }

        // Metoda rozwiązująca kolizję sprężystą między dwiema piłkami
        // Zakłada równe masy dla uproszczenia.
        private void ResolveBallCollision(Ball ball1, Ball ball2)
        {
            Vector2 pos1 = new Vector2(ball1.X, ball1.Y);
            Vector2 pos2 = new Vector2(ball2.X, ball2.Y);
            Vector2 vel1 = new Vector2(ball1.Vx, ball1.Vy);
            Vector2 vel2 = new Vector2(ball2.Vx, ball2.Vy);

            // Wektor normalny kolizji (kierunek od środka piłki 1 do środka piłki 2)
            Vector2 normal = Vector2.Normalize(pos2 - pos1);
            // Wektor styczny kolizji (prostopadły do normalnego)
            Vector2 tangent = new Vector2(-normal.Y, normal.X);

            // Rzut prędkości na wektor normalny i styczny
            float vel1Normal = Vector2.Dot(vel1, normal);
            float vel1Tangent = Vector2.Dot(vel1, tangent);
            float vel2Normal = Vector2.Dot(vel2, normal);
            float vel2Tangent = Vector2.Dot(vel2, tangent);

            // Wymiana prędkości wzdłuż wektora normalnego (dla kolizji sprężystej o równej masie)
            // Prędkości wzdłuż wektora stycznego pozostają niezmienione.
            float temp = vel1Normal;
            vel1Normal = vel2Normal;
            vel2Normal = temp;

            // Konwersja skalarnych prędkości z powrotem na wektory
            Vector2 newVel1 = vel1Normal * normal + vel1Tangent * tangent;
            Vector2 newVel2 = vel2Normal * normal + vel2Tangent * tangent;

            // Zaktualizuj prędkości piłek
            ball1.Vx = newVel1.X;
            ball1.Vy = newVel1.Y;
            ball2.Vx = newVel2.X;
            ball2.Vy = newVel2.Y;

            // Dodatkowe zabezpieczenie przed zaklinowaniem się piłek:
            // Jeśli piłki nadal się nakładają po rozwiązaniu kolizji (co może się zdarzyć
            // ze względu na dyskretyzację czasu symulacji), delikatnie je odsuń.
            float distance = Vector2.Distance(pos1, pos2);
            float overlap = (ball1.Radius + ball2.Radius) - distance;

            if (overlap > 0)
            {
                // Odsunięcie o połowę wartości nakładania wzdłuż wektora normalnego
                // Dodajemy małą wartość (np. 0.1f) do separacji, aby upewnić się, że piłki
                // nie zaczynają następnej klatki symulacji w stanie kolizji.
                Vector2 separation = normal * (overlap / 2f + 0.1f);
                ball1.X -= separation.X;
                ball1.Y -= separation.Y;
                ball2.X += separation.X;
                ball2.Y += separation.Y;

                // Powiadom o zmianie pozycji po odsunięciu, aby UI zareagowało
                ball1.OnPropertyChanged(nameof(ball1.X));
                ball1.OnPropertyChanged(nameof(ball1.Y));
                ball2.OnPropertyChanged(nameof(ball2.X));
                ball2.OnPropertyChanged(nameof(ball2.Y));
            }
        }
    }
}
