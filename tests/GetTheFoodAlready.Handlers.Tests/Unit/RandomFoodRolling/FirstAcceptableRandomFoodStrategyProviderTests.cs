using System;
using System.Diagnostics.CodeAnalysis;

using GetTheFoodAlready.Api.Orchestration.Requests;
using GetTheFoodAlready.Handlers.RandomFoodRolling;
using GetTheFoodAlready.Handlers.Tests.Support;
using GetTheFoodAlready.TestsCommon;

using Moq;

using NUnit.Framework;

namespace GetTheFoodAlready.Handlers.Tests.Unit.RandomFoodRolling
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class FirstAcceptableRandomFoodStrategyProviderTests
	{
		private IRandomFoodStrategyProvider _cut;

		private IRandomFoodStrategy _strat1;
		private IRandomFoodStrategy _strat2;
		private IRandomFoodStrategy _strat3;

		private readonly RandomFoodPropositionsRequest _randomFoodPropositionsRequest = TestDataFactory.CreateRequest();

		[SetUp]
		public void Setup()
		{
			_strat1 = Mock.Of<IRandomFoodStrategy>();
			_strat2 = Mock.Of<IRandomFoodStrategy>();
			_strat3 = Mock.Of<IRandomFoodStrategy>();

			_cut = new FirstAcceptableRandomFoodStrategyProvider(new[] {_strat1,_strat2,_strat3});
		}

		[Test]
		public void GetStrategy_FirstStrategyCanAccept_ReturnFirst()
		{
			//arrange
			_strat1.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(true);
			//act
			var result = _cut.GetStrategy(_randomFoodPropositionsRequest);
			//assert
			Assert.AreEqual(_strat1, result);
		}

		[Test]
		public void GetStrategy_AllStrategiesCanAccept_ReturnFirst()
		{
			//arrange
			_strat1.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(true);
			_strat2.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(true);
			_strat3.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(true);
			//act
			var result = _cut.GetStrategy(_randomFoodPropositionsRequest);
			//assert
			Assert.AreEqual(_strat1, result);
		}

		[Test]
		public void GetStrategy_LastStrategyCanAccept_ReturnLast()
		{
			//arrange
			_strat1.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(false);
			_strat2.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(false);
			_strat3.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(true);
			//act
			var result = _cut.GetStrategy(_randomFoodPropositionsRequest);
			//assert
			Assert.AreEqual(_strat3, result);
			_strat1.Verify(x => x.CanAccept(_randomFoodPropositionsRequest), Times.Once());
			_strat2.Verify(x => x.CanAccept(_randomFoodPropositionsRequest), Times.Once());
		}

		[Test]
		public void GetStrategy_AllStrategiesCannotAccept_Throws()
		{
			//arrange
			_strat1.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(false);
			_strat2.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(false);
			_strat3.Setup(x => x.CanAccept(_randomFoodPropositionsRequest))
				.Returns(false);
			//act
			//assert
			Assert.Throws<InvalidOperationException>(() => _cut.GetStrategy(_randomFoodPropositionsRequest));

		}
	}
}