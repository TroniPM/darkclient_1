namespace Mogo.AI.BT
{
	public sealed class BT1015 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT1015 _instance = null;
		public static BT1015 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT1015();

				return _instance;
			}
		}

		private BT1015()
		{
			{
				Mogo.AI.SelectorNode node1 = new Mogo.AI.SelectorNode();
				this.AddChild(node1);
				node1.AddChild(new Mogo.AI.CmpEnemyNum(Mogo.AI.CmpType.eq,0));
				{
					Mogo.AI.SequenceNode node3 = new Mogo.AI.SequenceNode();
					node1.AddChild(node3);
					{
						Mogo.AI.SelectorNode node4 = new Mogo.AI.SelectorNode();
						node3.AddChild(node4);
						node4.AddChild(new Mogo.AI.HasFightTarget());
						node4.AddChild(new Mogo.AI.AOI(30));
					}
					{
						Mogo.AI.Not node7 = new Mogo.AI.Not();
						node3.AddChild(node7);
						node7.Proxy(new Mogo.AI.ISCD());
					}
					node3.AddChild(new Mogo.AI.IsTargetCanBeAttack());
					{
						Mogo.AI.SelectorNode node10 = new Mogo.AI.SelectorNode();
						node3.AddChild(node10);
						{
							Mogo.AI.SequenceNode node11 = new Mogo.AI.SequenceNode();
							node10.AddChild(node11);
							node11.AddChild(new Mogo.AI.InSkillCoolDown(2));
							{
								Mogo.AI.SelectorNode node13 = new Mogo.AI.SelectorNode();
								node11.AddChild(node13);
								{
									Mogo.AI.SequenceNode node14 = new Mogo.AI.SequenceNode();
									node13.AddChild(node14);
									node14.AddChild(new Mogo.AI.InSkillRange(2));
									node14.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
                                    node14.AddChild(new Mogo.AI.CastSpell(2, 0));
									node14.AddChild(new Mogo.AI.EnterCD(0));
								}
								{
									Mogo.AI.SequenceNode node19 = new Mogo.AI.SequenceNode();
									node13.AddChild(node19);
									node19.AddChild(new Mogo.AI.ChooseCastPoint(2));
									node19.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
						{
							Mogo.AI.SequenceNode node22 = new Mogo.AI.SequenceNode();
							node10.AddChild(node22);
							node22.AddChild(new Mogo.AI.InSkillCoolDown(1));
							{
								Mogo.AI.SelectorNode node24 = new Mogo.AI.SelectorNode();
								node22.AddChild(node24);
								{
									Mogo.AI.SequenceNode node25 = new Mogo.AI.SequenceNode();
									node24.AddChild(node25);
									node25.AddChild(new Mogo.AI.InSkillRange(1));
									node25.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
                                    node25.AddChild(new Mogo.AI.CastSpell(1, 0));
									node25.AddChild(new Mogo.AI.EnterCD(0));
								}
								{
									Mogo.AI.SequenceNode node30 = new Mogo.AI.SequenceNode();
									node24.AddChild(node30);
									node30.AddChild(new Mogo.AI.ChooseCastPoint(1));
									node30.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
						{
							Mogo.AI.SelectorNode node33 = new Mogo.AI.SelectorNode();
							node10.AddChild(node33);
							{
								Mogo.AI.SequenceNode node34 = new Mogo.AI.SequenceNode();
								node33.AddChild(node34);
								node34.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
								node34.AddChild(new Mogo.AI.EnterRest(200));
							}
							{
								Mogo.AI.SequenceNode node37 = new Mogo.AI.SequenceNode();
								node33.AddChild(node37);
								node37.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
								node37.AddChild(new Mogo.AI.EnterRest(400));
							}
							node33.AddChild(new Mogo.AI.EnterRest(0));
						}
					}
				}
			}
		}
	}
}
