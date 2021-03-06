﻿using System.Collections.Generic;
using System.IO;

namespace NintendoLand.DataFormats
{
    /// <summary>
    /// The only public information shared between file parser and UI front end
    /// 
    /// This struct can be incomplete: Nintendo Land ships with 61 MapData*.exbin-files (50 in-game + 10 unused maps + tutorial)
    /// but only 50 rows inside StageData.exbin - this is required to be handled gracefully. Use .IsComplete to determine, if data is missing.
    /// All missing fields default to null, if you require more specific checks.
    /// </summary>
    public class Level
    {
        public StageData.Stage stage;
        public MapData mapData;
        public bool IsComplete => stage != null && mapData != null;
    }

    public class GameDataContainer
    {
        public StageData stageData;
        public FruitData fruitData;
        Dictionary<string, MapData> mapData = new Dictionary<string, MapData>(61); // 61 is based on the game's default amount of maps (50 in-game + 10 unused maps + tutorial)
        public Dictionary<string, MapData>.KeyCollection MapsAvailable => mapData.Keys;
        // @TODO VM FruitData
        // @TODO VM EnemyData

        /// <summary>
        /// Returns a struct containing the parsed contents of all related *Data.exbin-files of a level - can be incomplete see NintendoLand.DataFormats.Level.
        /// </summary>
        /// <param name="mapFileName"></param>
        /// <returns></returns>
        public Level GetLevelInfo(string mapFileName)
        {
            int _IDfromFilename = int.Parse(System.Text.RegularExpressions.Regex.Match(mapFileName, @"\d+").Value);
            return new Level
            {
                mapData = mapData[mapFileName],
                stage = stageData.HasLevelWithID(_IDfromFilename) ? stageData.GetLevelByID(_IDfromFilename) : StageData.Stage.CreateBlankDefault(_IDfromFilename)
                // @TODO VM EnemyData
            };
        }

        public GameDataContainer(string pathToYsiExtract)
        {
            stageData = StageData.Load(pathToYsiExtract);
            fruitData = FruitData.Load(pathToYsiExtract);

            IEnumerable<string> maps = Directory.EnumerateFiles(pathToYsiExtract, "MapData*.exbin");
            foreach (string map in maps)
            {
                mapData.Add(Path.GetFileName(map), MapData.Load(map));
            }
        }
    }
}
