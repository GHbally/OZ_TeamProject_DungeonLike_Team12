void CreateStageData(int chapter, int stage)
{
    if (chapter == 1 && stage == 1)
    {
        waves = new WaveData[]
        {
            new WaveData()
            {
                warriorCount = 3,
                archerCount = 0
            },

            new WaveData()
            {
                warriorCount = 5,
                archerCount = 1
            },

            new WaveData()
            {
                warriorCount = 7,
                archerCount = 2
            }
        };
    }
}
