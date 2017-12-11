namespace Mogo.AI.BT
{
	public sealed class BT2016 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT2016 _instance = null;
		public static BT2016 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT2016();

				return _instance;
			}
		}

		private BT2016()
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
							node11.AddChild(new Mogo.AI.InSkillCoolDown(7));
							node11.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
							node11.AddChild(new Mogo.AI.CastSpell(7,0));
							node11.AddChild(new Mogo.AI.EnterCD(200));
						}
						{
							Mogo.AI.SequenceNode node16 = new Mogo.AI.SequenceNode();
							node10.AddChild(node16);
							{
								Mogo.AI.Not node17 = new Mogo.AI.Not();
								node16.AddChild(node17);
								node17.Proxy(new Mogo.AI.InSkillCoolDown(1));
							}
							node16.AddChild(new Mogo.AI.InSkillRange(2));
							node16.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
							node16.AddChild(new Mogo.AI.Escape(1000));
						}
						{
							Mogo.AI.SequenceNode node22 = new Mogo.AI.SequenceNode();
							node10.AddChild(node22);
							{
								Mogo.AI.Not node23 = new Mogo.AI.Not();
								node22.AddChild(node23);
								node23.Proxy(new Mogo.AI.InSkillRange(1));
							}
							{
								Mogo.AI.SelectorNode node25 = new Mogo.AI.SelectorNode();
								node22.AddChild(node25);
								{
									Mogo.AI.SequenceNode node26 = new Mogo.AI.SequenceNode();
									node25.AddChild(node26);
									node26.AddChild(new Mogo.AI.InSkillCoolDown(6));
									node26.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,10));
									{
										Mogo.AI.SelectorNode node29 = new Mogo.AI.SelectorNode();
										node26.AddChild(node29);
										{
											Mogo.AI.SequenceNode node30 = new Mogo.AI.SequenceNode();
											node29.AddChild(node30);
											node30.AddChild(new Mogo.AI.InSkillRange(6));
											node30.AddChild(new Mogo.AI.CastSpell(6,0));
											node30.AddChild(new Mogo.AI.EnterCD(500));
										}
										{
											Mogo.AI.SequenceNode node34 = new Mogo.AI.SequenceNode();
											node29.AddChild(node34);
											node34.AddChild(new Mogo.AI.ChooseCastPoint(6));
											node34.AddChild(new Mogo.AI.MoveTo());
										}
									}
								}
								{
									Mogo.AI.SequenceNode node37 = new Mogo.AI.SequenceNode();
									node25.AddChild(node37);
									node37.AddChild(new Mogo.AI.InSkillCoolDown(4));
									{
										Mogo.AI.SelectorNode node39 = new Mogo.AI.SelectorNode();
										node37.AddChild(node39);
										{
											Mogo.AI.SequenceNode node40 = new Mogo.AI.SequenceNode();
											node39.AddChild(node40);
											node40.AddChild(new Mogo.AI.InSkillRange(4));
											node40.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
											node40.AddChild(new Mogo.AI.CastSpell(4,0));
											node40.AddChild(new Mogo.AI.EnterCD(-500));
										}
										{
											Mogo.AI.SequenceNode node45 = new Mogo.AI.SequenceNode();
											node39.AddChild(node45);
											node45.AddChild(new Mogo.AI.ChooseCastPoint(4));
											node45.AddChild(new Mogo.AI.MoveTo());
										}
									}
								}
								{
									Mogo.AI.SequenceNode node48 = new Mogo.AI.SequenceNode();
									node25.AddChild(node48);
									node48.AddChild(new Mogo.AI.InSkillCoolDown(3));
									{
										Mogo.AI.SelectorNode node50 = new Mogo.AI.SelectorNode();
										node48.AddChild(node50);
										{
											Mogo.AI.SequenceNode node51 = new Mogo.AI.SequenceNode();
											node50.AddChild(node51);
											node51.AddChild(new Mogo.AI.InSkillRange(3));
											node51.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
											node51.AddChild(new Mogo.AI.CastSpell(3,0));
											node51.AddChild(new Mogo.AI.EnterCD(500));
										}
										{
											Mogo.AI.SequenceNode node56 = new Mogo.AI.SequenceNode();
											node50.AddChild(node56);
											node56.AddChild(new Mogo.AI.ChooseCastPoint(3));
											node56.AddChild(new Mogo.AI.MoveTo());
										}
									}
								}
							}
						}
						{
							Mogo.AI.SequenceNode node59 = new Mogo.AI.SequenceNode();
							node10.AddChild(node59);
							node59.AddChild(new Mogo.AI.InSkillRange(2));
							{
								Mogo.AI.SelectorNode node61 = new Mogo.AI.SelectorNode();
								node59.AddChild(node61);
								{
									Mogo.AI.SequenceNode node62 = new Mogo.AI.SequenceNode();
									node61.AddChild(node62);
									node62.AddChild(new Mogo.AI.InSkillCoolDown(5));
									node62.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
									node62.AddChild(new Mogo.AI.CastSpell(5,0));
									node62.AddChild(new Mogo.AI.EnterCD(500));
								}
								{
									Mogo.AI.SequenceNode node67 = new Mogo.AI.SequenceNode();
									node61.AddChild(node67);
									node67.AddChild(new Mogo.AI.InSkillCoolDown(2));
									node67.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
									node67.AddChild(new Mogo.AI.CastSpell(2,0));
									node67.AddChild(new Mogo.AI.EnterCD(500));
								}
							}
						}
						{
							Mogo.AI.SequenceNode node72 = new Mogo.AI.SequenceNode();
							node10.AddChild(node72);
							node72.AddChild(new Mogo.AI.InSkillCoolDown(1));
							{
								Mogo.AI.SelectorNode node74 = new Mogo.AI.SelectorNode();
								node72.AddChild(node74);
								{
									Mogo.AI.SequenceNode node75 = new Mogo.AI.SequenceNode();
									node74.AddChild(node75);
									node75.AddChild(new Mogo.AI.InSkillRange(1));
									node75.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
									node75.AddChild(new Mogo.AI.CastSpell(1,0));
									node75.AddChild(new Mogo.AI.EnterCD(500));
								}
								{
									Mogo.AI.SequenceNode node80 = new Mogo.AI.SequenceNode();
									node74.AddChild(node80);
									node80.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
									node80.AddChild(new Mogo.AI.ChooseCastPoint(1));
									node80.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
						node10.AddChild(new Mogo.AI.EnterRest(200));
					}
				}
			}
		}
	}
}
