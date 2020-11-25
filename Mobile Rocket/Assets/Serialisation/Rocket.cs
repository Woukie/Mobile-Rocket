using System;
using System.Collections.Generic;
using Assets.Rocket.Parts;

namespace Assets.Serialisation
{
    [Serializable]
    public struct Rocket
    {
        public string Name;
        public List<Part> Parts;

        public Rocket(string name, List<Part> parts)
        {
            Name = name;
            Parts = parts;
        }
    }
}