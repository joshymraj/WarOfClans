using UnityEngine;
using UnityEngine.Playables;

namespace WarOfClans
{
	public class CinematicsManager : MonoBehaviour
	{
		public PlayableDirector redTowerDestructionCinematics;
		public PlayableDirector blueTowerDestructionCinematics;

		[SerializeField] private UIEventChannel uiEventChannel;

		private Animator animator;
		private CameraView cameraView;

        private void Awake()
        {
			animator = GetComponent<Animator>();
			cameraView = CameraView.IsometricView;

			SubscribeEvents();
        }

		private void SubscribeEvents()
        {
			if(uiEventChannel != null)
            {
				uiEventChannel.OnCameraSwitchTapped += SwitchCamera;
            }
		}

		public void SwitchCamera()
        {
			cameraView = cameraView == CameraView.IsometricView ? CameraView.TopView : CameraView.IsometricView;
			animator.SetBool("Isometric", cameraView == CameraView.IsometricView);
        }

        public void PlayCollapseCutscene(Clan losingClan)
		{
			switch(losingClan)
            {
				case Clan.Red:
					redTowerDestructionCinematics.Play();
					break;

				case Clan.Blue:
					blueTowerDestructionCinematics.Play();
					break;

				default:
					break;
            }
		}


	}
}