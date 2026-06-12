namespace Features.Dogs.Models
{
    public struct DogBreedViewData
    {
        public readonly string m_id;
        public readonly string m_name;

        public DogBreedViewData(string _id, string _name)
        {
            m_id = _id;
            m_name = _name;
        }
    }
}