using UnityEngine;

namespace Features.Popup
{
    public class DebugPopupService: IPopupService
    {
        public void ShowInfo(string _title, string _description)
        {
            Debug.Log($"[Popup] {_title}\n{_description}");
        }

        public void Hide()
        {
        }
    }
}