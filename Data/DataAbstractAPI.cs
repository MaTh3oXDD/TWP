using System;

namespace Data
{
    public abstract class DataAbstractAPI
    {
        public static DataAbstractAPI CreateApi()
        {
            return new DataAPI();
        }

        // Metoda abstrakcyjna do logowania stanu kuli
        public abstract void LogBallState(float x, float y, float vx, float vy, float timestamp);

        // (opcjonalnie) jeśli chcesz odpowiednio zamknąć logger
        public abstract void StopLogger();
    }
}
