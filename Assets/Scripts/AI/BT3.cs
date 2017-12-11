namespace Mogo.AI.BT
{
	public sealed class BT3 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT3 _instance = null;
		public static BT3 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT3();

				return _instance;
			}
		}

		private BT3()
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
							{
								Mogo.AI.Not node12 = new Mogo.AI.Not();
								node11.AddChild(node12);
								node12.Proxy(new Mogo.AI.InSkillRange(1));
							}
							node11.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,100));
                            node11.AddChild(new Mogo.AI.LookOn(500, 200, 1000, 50, 1000, 50, 1000, 50, 1000, 50, 1000, 0, 50, 1));
						}
						{
							Mogo.AI.SequenceNode node16 = new Mogo.AI.SequenceNode();
							node10.AddChild(node16);
							{
								Mogo.AI.Not node17 = new Mogo.AI.Not();
								node16.AddChild(node17);
								node17.Proxy(new Mogo.AI.InSkillCoolDown(1));
							}
							node16.AddChild(new Mogo.AI.InSkillRange(1));
							node16.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
                            node16.AddChild(new Mogo.AI.LookOn(500, 200, 1000, 50, 1000, 50, 1000, 50, 1000, 50, 1000, 0, 50, 1));
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
									node25.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,40));
                                    node25.AddChild(new Mogo.AI.CastSpell(1, 0));
									node25.AddChild(new Mogo.AI.EnterCD(0));
								}
								{
									Mogo.AI.SelectorNode node30 = new Mogo.AI.SelectorNode();
									node24.AddChild(node30);
									{
										Mogo.AI.SequenceNode node31 = new Mogo.AI.SequenceNode();
										node30.AddChild(node31);
										node31.AddChild(new Mogo.AI.InSkillRange(1));
										node31.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
                                        node31.AddChild(new Mogo.AI.LookOn(500, 200, 1000, 50, 1000, 50, 1000, 50, 1000, 50, 1000, 0, 50, 1));
									}
									{
										Mogo.AI.SequenceNode node35 = new Mogo.AI.SequenceNode();
										node30.AddChild(node35);
										{
											Mogo.AI.Not node36 = new Mogo.AI.Not();
											node35.AddChild(node36);
											node36.Proxy(new Mogo.AI.InSkillRange(1));
										}
										node35.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
                                        node35.AddChild(new Mogo.AI.LookOn(500, 200, 1000, 50, 1000, 50, 1000, 50, 1000, 50, 1000, 0, 50, 1));
									}
								}
							}
						}
						node10.AddChild(new Mogo.AI.EnterRest(400));
					}
				}
			}
		}
	}
}
