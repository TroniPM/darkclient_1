namespace Mogo.AI.BT
{
	public sealed class BT3017 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT3017 _instance = null;
		public static BT3017 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT3017();

				return _instance;
			}
		}

		private BT3017()
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
							node11.AddChild(new Mogo.AI.InSkillCoolDown(4));
							{
								Mogo.AI.SequenceNode node13 = new Mogo.AI.SequenceNode();
								node11.AddChild(node13);
								node13.AddChild(new Mogo.AI.InSkillRange(4));
								node13.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
                                node13.AddChild(new Mogo.AI.CastSpell(4, 0));
								node13.AddChild(new Mogo.AI.EnterCD(0));
							}
						}
						{
							Mogo.AI.SequenceNode node18 = new Mogo.AI.SequenceNode();
							node10.AddChild(node18);
							node18.AddChild(new Mogo.AI.InSkillCoolDown(3));
							{
								Mogo.AI.SequenceNode node20 = new Mogo.AI.SequenceNode();
								node18.AddChild(node20);
								node20.AddChild(new Mogo.AI.InSkillRange(3));
								node20.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
                                node20.AddChild(new Mogo.AI.CastSpell(3, 0));
								node20.AddChild(new Mogo.AI.EnterCD(0));
							}
						}
						{
							Mogo.AI.SequenceNode node25 = new Mogo.AI.SequenceNode();
							node10.AddChild(node25);
							node25.AddChild(new Mogo.AI.InSkillCoolDown(2));
							{
								Mogo.AI.SequenceNode node27 = new Mogo.AI.SequenceNode();
								node25.AddChild(node27);
								node27.AddChild(new Mogo.AI.InSkillRange(2));
								node27.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
                                node27.AddChild(new Mogo.AI.CastSpell(2, 0));
								node27.AddChild(new Mogo.AI.EnterCD(0));
							}
						}
						{
							Mogo.AI.SequenceNode node32 = new Mogo.AI.SequenceNode();
							node10.AddChild(node32);
							node32.AddChild(new Mogo.AI.InSkillCoolDown(1));
							{
								Mogo.AI.SequenceNode node34 = new Mogo.AI.SequenceNode();
								node32.AddChild(node34);
								node34.AddChild(new Mogo.AI.InSkillRange(1));
								node34.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,80));
                                node34.AddChild(new Mogo.AI.CastSpell(1, 0));
								node34.AddChild(new Mogo.AI.EnterCD(0));
							}
							{
								Mogo.AI.SequenceNode node39 = new Mogo.AI.SequenceNode();
								node32.AddChild(node39);
								node39.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,80));
								node39.AddChild(new Mogo.AI.ChooseCastPoint(1));
								node39.AddChild(new Mogo.AI.MoveTo());
							}
						}
						node10.AddChild(new Mogo.AI.EnterRest(200));
					}
				}
			}
		}
	}
}
