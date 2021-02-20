using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NintendoLand.DataFormats;
using NintendoLand.TileTypes;
using tileeditor.Annotations;
using tileeditor.GridObjects;

namespace tileeditor.DataFormats
{
    class FocusableComponent<T> : INotifyPropertyChanged
    {
        public T mainValue { get; }
        public bool isFocussed = false;

        public bool IsFocussed
        {
            get => isFocussed;
            private set
            {
                isFocussed = value;
                OnPropertyChanged();
            }
        }

        public FocusableComponent(T value)
        {
            mainValue = value;
        }

        public void SetFocus(bool newFocus)
        {
            IsFocussed = newFocus;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class MapDescriptor
    {
        public byte mapID;
        public byte[] tutorialText1;
        public byte[] tutorialText2;
        public NintendoLand.DataFormats.IndexResolver backgroundID;
        public GridObjects.BaseObject[,] grid;
        public FocusableComponent<StageData.Stage.PepperOrSwitchFlag>[] pepperOrSwitchFlagMap;
        public FocusableComponent<IndexResolver>[] movementPatternMap;
        public FocusableComponent<IndexResolver>[] fruitOrderMap;
        public FocusableComponent<IndexResolver>[] fruitAssociations;

        private MapDescriptor() { }

        #region Conversion
        public static NintendoLand.DataFormats.MapData ToGameData(MapDescriptor descriptor)
        {
            throw new System.NotImplementedException();
        }

        public static MapDescriptor FromGameData(string fileName, NintendoLand.DataFormats.GameDataContainer gameDataContainer)
        {
            NintendoLand.DataFormats.Level level = gameDataContainer.GetLevelInfo(fileName);
            MapDescriptor mapDescriptor = new MapDescriptor
            {
                mapID = level.stage.ID,
                backgroundID = level.stage.backgroundID,
                tutorialText1 = level.stage.tutorialText1,
                tutorialText2 = level.stage.tutorialText2,
                grid = new BaseObject[NintendoLand.DataFormats.MapData.ROWS_TOTAL, NintendoLand.DataFormats.MapData.COLUMNS_TOTAL],
                pepperOrSwitchFlagMap = level.stage.switchOrPepperDefinitions.Select(entry => new FocusableComponent<StageData.Stage.PepperOrSwitchFlag>(entry)).ToArray(),
                movementPatternMap = level.stage.movementPatterns.Select(entry =>  new FocusableComponent<IndexResolver>(entry)).ToArray(),
                fruitOrderMap = level.stage.orderedFruitDefinition.Select(entry => new FocusableComponent<IndexResolver>(entry)).ToArray(),
                fruitAssociations = level.stage.fruitAssociations.Select(entry =>  new FocusableComponent<IndexResolver>(entry)).ToArray()
            };
            for (int row = 0; row < NintendoLand.DataFormats.MapData.ROWS_TOTAL; row++)
            {
                for (int column = 0; column < NintendoLand.DataFormats.MapData.COLUMNS_TOTAL; column++)
                {
                    NintendoLand.TileTypes.BaseType type = level.mapData.GetItem(row, column);
                    GridObjects.BaseObject createdObject = null;
                    GridObjects.Registrar.ForEach((GridObjects.BaseObject baseType) =>
                    {
                        if (baseType.CanConvert(type, level.stage))
                        {
                            createdObject = baseType.FromTileType(type, level.stage);
                        }
                    });
                    System.Diagnostics.Debug.Assert(
                        createdObject != null,
                        "No valid GridObject conversion",
                        $"No GridObject could be created from tile [{row}, {column}]: {type}"
                    );
                    createdObject.SetPosition(column, row);
                    mapDescriptor.grid[row, column] = createdObject;
                }
            }

            return mapDescriptor;
        }
        #endregion
    }
}
