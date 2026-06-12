namespace Features.Dogs.Models
{
    public struct DogBreedDetailsViewData
    {
        public readonly string m_id;
        public readonly string m_name;
        public readonly string m_description;

        public DogBreedDetailsViewData(
            string _id,
            string _name,
            string _description)
        {
            m_id = _id;
            m_name = _name;
            m_description = _description;
        }
    }
}