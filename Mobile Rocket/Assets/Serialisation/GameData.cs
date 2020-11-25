using System.Collections.Generic;

namespace Assets.Serialisation
{
    public struct GameData
    {
        public string Name;
        public List<Rocket> Rockets;

        public GameData(string name, List<Rocket> rockets)
        {
            Name = name;
            Rockets = rockets;
        }

        public void SaveData()
        {

        }
    }
}
