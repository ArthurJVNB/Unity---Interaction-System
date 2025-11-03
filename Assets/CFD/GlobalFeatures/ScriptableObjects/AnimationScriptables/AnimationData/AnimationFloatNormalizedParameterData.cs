using System;
using System.ComponentModel;
using UnityEngine;

namespace CFD
{
	[CreateAssetMenu(fileName = "New AnimationFloatNormalizedParameterData", menuName = BasePath + "Float/Float (Normalized)")]
	public class AnimationFloatNormalizedParameterData : AAnimationFloatParameterData
	{
		[SerializeField] private float _value;
		[SerializeField] private Vector2 _valueRange;
		[SerializeField] private Vector2 _valueNormalizedRange = new(-1, 1);
		[SerializeField] private NormalizationType _normalizationType = NormalizationType.Unclamped;

		public float Value => _value;
		public Vector2 ValueRange => _valueRange;
		public Vector2 ValueNormalizedRange => _valueNormalizedRange;
		public NormalizationType Normalization => _normalizationType;
		public float ValueNormalized => Normalize(_value, _valueRange, _valueNormalizedRange, _normalizationType);

		public enum NormalizationType
		{
			Clamped,
			Unclamped,
		}

		public override void Apply(Animator animator)
		{
			Apply(animator, _value);
		}

		/// <summary>
		/// Applies the float value into the animator. It will be normalized internally.
		/// </summary>
		/// <param name="animator">Animator to apply to.</param>
		/// <param name="value">Not normalized value. It will be normalized internally.</param>
		public override void Apply(Animator animator, float value)
		{
			base.Apply(animator, Normalize(value, _valueRange, _valueNormalizedRange, _normalizationType));
		}

		private static float Normalize(float value, Vector2 fromRange, Vector2 toRange, NormalizationType type)
		{
			float normalized = (toRange.y - toRange.x) * (value - fromRange.x) / (fromRange.y - fromRange.x) + toRange.x;
			return type switch
			{
				NormalizationType.Clamped => Mathf.Clamp(normalized, toRange.x, toRange.y),
				NormalizationType.Unclamped => normalized,
				_ => throw new InvalidEnumArgumentException("Invalid normalization type value", (int)type, typeof(NormalizationType)),
			};
		}
	}
}
