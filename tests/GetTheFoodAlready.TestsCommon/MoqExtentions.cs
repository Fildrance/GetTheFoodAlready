using System;
using System.Linq.Expressions;

using Moq;
using Moq.Language;
using Moq.Language.Flow;

namespace GetTheFoodAlready.TestsCommon
{
    public static class MoqExtentions
    {
	    public static ISetup<T, TResult> Setup<T, TResult>(this T subject, Expression<Func<T, TResult>> expectedAction) where T : class
	    {
		    return Mock.Get(subject).Setup(expectedAction);
	    }

	    public static IReturnsResult<T> Setup<T, TResult>(this T subject, Expression<Func<T, TResult>> expectedAction, TResult expectedResult) where T : class
	    {
		    return Mock.Get(subject).Setup(expectedAction).Returns(expectedResult);
	    }

	    public static ISetup<T> Setup<T>(this T subject, Expression<Action<T>> expectedAction) where T : class
	    {
		    return Mock.Get(subject).Setup(expectedAction);
	    }

	    public static void VerifyAll<T>(this T subject) where T : class
	    {
		    Mock.Get(subject).VerifyAll();
	    }

	    public static void DropSetup<T>(this T subject) where T : class
	    {
		    Mock.Get(subject).Reset();
	    }

	    public static void Verify<T>(this T subject, Expression<Action<T>> action, Times times = default(Times)) where T : class
	    {
		    if (times == new Times())
			    times = Times.AtMostOnce();
		    Mock.Get(subject).Verify(action, () => times);
	    }

	    public static void VerifyGet<T, TProp>(this T subject, Expression<Func<T, TProp>> action, Times times = default(Times)) where T : class
	    {
		    if (times == new Times())
			    times = Times.AtMostOnce();
		    Mock.Get(subject).VerifyGet(action, () => times);
	    }

	    public static ISetupSequentialResult<TResult> SetupSequence<T, TResult>(this T subject, Expression<Func<T,TResult>> expectedAction) where T : class
	    {
		    return Mock.Get(subject)
			    .SetupSequence(expectedAction);
	    }
	}
}
