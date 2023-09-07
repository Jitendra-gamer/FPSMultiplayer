using System;
using UnityEngine;
using TMPro;

namespace FPS
{
	public class UIManager : MonoBehaviour
	{
		#region SerializeField

		private PlayerController player;

        [SerializeField] private TMP_Text equippedWeapon_Name;
        [SerializeField] private TMP_Text equippedWeapon_ClipAmmo;
        [SerializeField] private TMP_Text equippedWeapon_TotalAmmo;

        /* [SerializeField] private TMP_Text primaryWeaponFirst_Name;
         [SerializeField] private TMP_Text primaryWeaponFirst_ClipAmmo;
         [SerializeField] private TMP_Text primaryWeaponFirst_TotalAmmo;
         [SerializeField] private TMP_Text primaryWeaponSecond_Name;
         [SerializeField] private TMP_Text primaryWeaponSecond_ClipAmmo;
         [SerializeField] private TMP_Text primaryWeaponSecond_TotalAmmo;
         [SerializeField] private TMP_Text secondaryWeaponFirst_Name;
         [SerializeField] private TMP_Text secondaryWeaponFirst_ClipAmmo;
         [SerializeField] private TMP_Text secondaryWeaponFirst_TotalAmmo;
        */

        #endregion

        #region Unity Methods

        public void SetPlayer(PlayerController player)
		{
			this.player = player;
		}

		void OnEnable()
		{
            PlayerController.UpdateUIEvent += UpdateUIText;
		}

        void OnDisable()
		{
			PlayerController.UpdateUIEvent -= UpdateUIText;
		}

		#endregion

		#region private Methods

		private void UpdateUIText()
		{
            if (player.EquippedWeapon != null)
            {
                equippedWeapon_Name.text = player.EquippedWeapon.WeaponName;
                equippedWeapon_ClipAmmo.text = player.EquippedWeapon.AmmoInMagzine.ToString();
                equippedWeapon_TotalAmmo.text = (player.EquippedWeapon.TotalAmmo + player.EquippedWeapon.AmmoInMagzine).ToString();
            }

           /* if (player.PrimaryWeaponFirst != null)
			{
				primaryWeaponFirst_Name.text = player.PrimaryWeaponFirst.WeaponName;
				primaryWeaponFirst_ClipAmmo.text = player.PrimaryWeaponFirst.AmmoInMagzine.ToString();
				primaryWeaponFirst_TotalAmmo.text = (player.PrimaryWeaponFirst.TotalAmmo + player.PrimaryWeaponFirst.AmmoInMagzine).ToString();
			}

			if (player.PrimaryWeaponSecond != null)
			{
				primaryWeaponSecond_Name.text = player.PrimaryWeaponSecond.WeaponName;
				primaryWeaponSecond_ClipAmmo.text = player.PrimaryWeaponSecond.AmmoInMagzine.ToString();
				primaryWeaponSecond_TotalAmmo.text = (player.PrimaryWeaponSecond.TotalAmmo + player.PrimaryWeaponSecond.AmmoInMagzine).ToString();
			}

			if (player.SecondaryWeaponFirst != null)
			{
				secondaryWeaponFirst_Name.text = player.SecondaryWeaponFirst.WeaponName;
				secondaryWeaponFirst_ClipAmmo.text = player.SecondaryWeaponFirst.AmmoInMagzine.ToString();
				secondaryWeaponFirst_TotalAmmo.text = (player.SecondaryWeaponFirst.TotalAmmo + player.SecondaryWeaponFirst.AmmoInMagzine).ToString();
			}*/
		}

		#endregion
	}
}
