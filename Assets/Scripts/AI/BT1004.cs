namespace Mogo.AI.BT
{
	public sealed class BT1004 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT1004 _instance = null;
		public static BT1004 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT1004();

				return _instance;
			}
		}

		private BT1004()
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
									node14.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,10));
                                    node14.AddChild(new Mogo.AI.CastSpell(2, 0));
									node14.AddChild(new Mogo.AI.EnterCD(0));
								}
								{
									Mogo.AI.SequenceNode node19 = new Mogo.AI.SequenceNode();
									node13.AddChild(node19);
									node19.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
									node19.AddChild(new Mogo.AI.ChooseCastPoint(2));
									node19.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
						{
							Mogo.AI.SequenceNode node23 = new Mogo.AI.SequenceNode();
							node10.AddChild(node23);
							node23.AddChild(new Mogo.AI.InSkillCoolDown(1));
							{
								Mogo.AI.SelectorNode node25 = new Mogo.AI.SelectorNode();
								node23.AddChild(node25);
								{
									Mogo.AI.SequenceNode node26 = new Mogo.AI.SequenceNode();
									node25.AddChild(node26);
									node26.AddChild(new Mogo.AI.InSkillRange(1));
									node26.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
                                    node26.AddChild(new Mogo.AI.CastSpell(1, 0));
									node26.AddChild(new Mogo.AI.EnterCD(0));
								}
								{
									Mogo.AI.SequenceNode node31 = new Mogo.AI.SequenceNode();
									node25.AddChild(node31);
									node31.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
									node31.AddChild(new Mogo.AI.ChooseCastPoint(1));
									node31.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
						{
							Mogo.AI.SelectorNode node35 = new Mogo.AI.SelectorNode();
							node10.AddChild(node35);
							{
								Mogo.AI.SequenceNode node36 = new Mogo.AI.SequenceNode();
								node35.AddChild(node36);
								node36.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,20));
								node36.AddChild(new Mogo.AI.EnterRest(1200));
							}
							{
								Mogo.AI.SequenceNode node39 = new Mogo.AI.SequenceNode();
								node35.AddChild(node39);
								node39.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,20));
								node39.AddChild(new Mogo.AI.EnterRest(1000));
							}
							{
								Mogo.AI.SequenceNode node42 = new Mogo.AI.SequenceNode();
								node35.AddChild(node42);
								node42.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,20));
								node42.AddChild(new Mogo.AI.EnterRest(800));
							}
							{
								Mogo.AI.SequenceNode node45 = new Mogo.AI.SequenceNode();
								node35.AddChild(node45);
								node45.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,20));
								node45.AddChild(new Mogo.AI.EnterRest(600));
							}
							{
								Mogo.AI.SequenceNode node48 = new Mogo.AI.SequenceNode();
								node35.AddChild(node48);
								node48.AddChild(new Mogo.AI.ChooseCastPoint(1));
								node48.AddChild(new Mogo.AI.MoveTo());
							}
							node35.AddChild(new Mogo.AI.EnterRest(400));
						}
					}
				}
			}
		}
	}
}
