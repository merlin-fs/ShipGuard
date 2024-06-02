using UnityEngine;

namespace Common.UI.Windows
{
	public interface IWindowAnimator
	{
		IAnimation Animate(IWindow.AnimationMode mode, float time);
	}

	public abstract class WindowAnimator : MonoBehaviour, IWindowAnimator
	{
		IAnimation IWindowAnimator.Animate(IWindow.AnimationMode mode, float time) => Animate(mode, time);
		protected abstract IAnimation Animate(IWindow.AnimationMode mode, float time);
	}
}