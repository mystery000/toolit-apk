﻿using System;
using Xamarin.CommunityToolkit.Converters;

namespace Toolit.Converters.MathExpressionConverterHelpers
{
	sealed class MathOperator
	{
		public string Name { get; }

		public int NumericCount { get; }

		public MathOperatorPrecedence Precedence { get; }

		public Func<double[], double> CalculateFunc { get; }

		public MathOperator(
			string name,
			int numericCount,
			MathOperatorPrecedence precedence,
			Func<double[], double> calculateFunc)
		{
			Name = name;
			CalculateFunc = calculateFunc;
			Precedence = precedence;
			NumericCount = numericCount;
		}
	}
}