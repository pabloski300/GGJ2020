using UnityEngine;
using UnityEngine.UI;

public class Ammobar: MonoBehaviour {
    [SerializeField]
    private Image image;
    public void Start() {
        Turret turret = GetComponentInParent<Turret>();
        
        turret.changeAmmoEvent.AddListener(UpdateAmmo);
    }

    public void UpdateAmmo(float amountRelative){
        
        image.fillAmount = amountRelative;
    }
}