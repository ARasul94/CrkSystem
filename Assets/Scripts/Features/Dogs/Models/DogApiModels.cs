using System;

namespace Features.Dogs.Models
{
    [Serializable]
    public class DogBreedsResponse
    {
        public DogBreedData[] data;
    }
    
    [Serializable]
    public class DogBreedDetailsResponse
    {
        public DogBreedData data;
    }
    
    [Serializable]
    public class DogBreedData
    {
        public string id;
        public string type;
        public DogBreedAttributes attributes;
    }
    
    [Serializable]
    public class DogBreedAttributes
    {
        public string name;
        public string description;
    }
}