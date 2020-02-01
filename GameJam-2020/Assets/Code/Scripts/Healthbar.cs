using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour {
    [SerializeField]
    private Image image;
    public void Start() {
        Turret turret = GetComponentInParent<Turret>();
        turret.changeHealthEvent.AddListener(UpdateHealth);
    }

    public void UpdateHealth(float amountRelative){
        image.fillAmount = amountRelative;
    }
}