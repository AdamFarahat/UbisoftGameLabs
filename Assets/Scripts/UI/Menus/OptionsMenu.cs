using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject[] listOfRowParameters;
    [SerializeField] private float ScrollDelta = 10f;
    [SerializeField] private GameObject ScrollContainer;

    private int previousRowIndex = 0;
    private int rowIndex = 0;
    

    private OptionsMenu instance;
    private RectTransform scrollTransform;
    private GameObject SelectedParameterRow => listOfRowParameters[rowIndex];

    private GameObject PreviousSelectedParameterRow => listOfRowParameters[previousRowIndex];

    public OptionsMenu Instance => instance;
     private void Awake()
    {
        
        Assert.IsNotNull(ScrollContainer);
        scrollTransform = ScrollContainer.GetComponent<RectTransform>();

        Assert.IsNotNull(scrollTransform);
        Assert.IsNotNull(listOfRowParameters);
        instance = this;
    }
    private void OnEnable()
    {
        
    }
    public void OnMove(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();

            if (input.y > 0.5f) IncrementCursor();        // Move Up
            else if (input.y < -0.5f) DecrementCursor(); // Move Down
            else UpdateSetting(input.x);
        }
    }

    private void UpdateSetting(float x)
    {
        Slider s = SelectedParameterRow.GetComponentInChildren<Slider>();
        if (s != null)
        {
            s.value += x * s.maxValue * 0.01f;
        }
        else {
            //TODO: implement other types of settings (e.g. dropdowns, toggles, etc.)

        }
    }

    private void DecrementCursor()
    {
        if (rowIndex <= 0) {
            return;
        }

        previousRowIndex = rowIndex--;
        scrollTransform.position.Set(scrollTransform.position.x, scrollTransform.position.y - ScrollDelta, scrollTransform.position.z);
        UpdateHighlight();
    }
    private void IncrementCursor()
    {
        if (rowIndex >= listOfRowParameters.Length - 1)
        {
            return;
        }
        previousRowIndex = rowIndex++;

        scrollTransform.position.Set(scrollTransform.position.x, scrollTransform.position.y + ScrollDelta, scrollTransform.position.z);
        UpdateHighlight();

    }
    private void UpdateHighlight()
    {
        if (SelectedParameterRow.TryGetComponent(out Image currentBg))
        {
            setAlpha(currentBg, 1f);
        }
        if (PreviousSelectedParameterRow.TryGetComponent(out Image previousBg))
        {
            setAlpha(previousBg, 0f);
        }
    }

    private void setAlpha(Image bg, float alpha)
    {
        Color temp = bg.color;
        temp.a = alpha;
        bg.color = temp;
    }


    public void OnVolumeChange(float value) { 

    }

    
}
