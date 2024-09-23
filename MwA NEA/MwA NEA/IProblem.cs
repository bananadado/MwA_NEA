using System;

namespace MwA_NEA
{
	public interface IProblem
	{
		void GenerateQuestion(Random rnd);
		void GenerateAnswer();
		void InputQuestion();
	}
}
