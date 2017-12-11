namespace Mogo.AI.BT
{
	public sealed class BT1037 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT1037 _instance = null;
		public static BT1037 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT1037();

				return _instance;
			}
		}

		private BT1037()
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
						node4.AddChild(new Mogo.AI.AOI(0));
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
							node11.AddChild(new Mogo.AI.LastLookOnModeIs(5));
							node11.AddChild(new Mogo.AI.CastSpell(0,0));
							node11.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node15 = new Mogo.AI.SequenceNode();
							node10.AddChild(node15);
							node15.AddChild(new Mogo.AI.InSkillCoolDown(2));
							{
								Mogo.AI.SelectorNode node17 = new Mogo.AI.SelectorNode();
								node15.AddChild(node17);
								{
									Mogo.AI.SequenceNode node18 = new Mogo.AI.SequenceNode();
									node17.AddChild(node18);
									node18.AddChild(new Mogo.AI.InSkillRange(2));
									node18.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
									node18.AddChild(new Mogo.AI.CastSpell(2,0));
									node18.AddChild(new Mogo.AI.EnterCD(-1000));
								}
								{
									Mogo.AI.SequenceNode node23 = new Mogo.AI.SequenceNode();
									node17.AddChild(node23);
									{
										Mogo.AI.Not node24 = new Mogo.AI.Not();
										node23.AddChild(node24);
										node24.Proxy(new Mogo.AI.InSkillRange(2));
									}
									node23.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,60));
									node23.AddChild(new Mogo.AI.ChooseCastPoint(2));
									node23.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
						{
							Mogo.AI.SequenceNode node29 = new Mogo.AI.SequenceNode();
							node10.AddChild(node29);
							node29.AddChild(new Mogo.AI.InSkillCoolDown(1));
							{
								Mogo.AI.SelectorNode node31 = new Mogo.AI.SelectorNode();
								node29.AddChild(node31);
								{
									Mogo.AI.SequenceNode node32 = new Mogo.AI.SequenceNode();
									node31.AddChild(node32);
									node32.AddChild(new Mogo.AI.InSkillRange(1));
									node32.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,70));
									node32.AddChild(new Mogo.AI.CastSpell(1,0));
									node32.AddChild(new Mogo.AI.EnterCD(0));
								}
								{
									Mogo.AI.SequenceNode node37 = new Mogo.AI.SequenceNode();
									node31.AddChild(node37);
									{
										Mogo.AI.Not node38 = new Mogo.AI.Not();
										node37.AddChild(node38);
										node38.Proxy(new Mogo.AI.InSkillRange(1));
									}
									{
										Mogo.AI.SelectorNode node40 = new Mogo.AI.SelectorNode();
										node37.AddChild(node40);
										{
											Mogo.AI.SequenceNode node41 = new Mogo.AI.SequenceNode();
											node40.AddChild(node41);
											node41.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,10));
											node41.AddChild(new Mogo.AI.LookOn(500,250,0,0,0,0,0,0,0,0,0,0,100,1));
										}
										{
											Mogo.AI.SequenceNode node44 = new Mogo.AI.SequenceNode();
											node40.AddChild(node44);
											node44.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,40));
											node44.AddChild(new Mogo.AI.ChooseCastPoint(1));
											node44.AddChild(new Mogo.AI.MoveTo());
										}
									}
								}
							}
						}
						node10.AddChild(new Mogo.AI.LookOn(500,350,500,40,1000,25,1000,10,1000,10,1000,25,0,1));
					}
				}
			}
		}
	}
}
