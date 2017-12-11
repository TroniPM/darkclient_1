namespace Mogo.AI.BT
{
	public sealed class BT2009 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT2009 _instance = null;
		public static BT2009 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT2009();

				return _instance;
			}
		}

		private BT2009()
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
							node11.AddChild(new Mogo.AI.InSkillCoolDown(6));
							{
								Mogo.AI.Not node13 = new Mogo.AI.Not();
								node11.AddChild(node13);
								node13.Proxy(new Mogo.AI.InSkillRange(1));
							}
							node11.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,15));
							{
								Mogo.AI.SelectorNode node16 = new Mogo.AI.SelectorNode();
								node11.AddChild(node16);
								{
									Mogo.AI.SequenceNode node17 = new Mogo.AI.SequenceNode();
									node16.AddChild(node17);
									node17.AddChild(new Mogo.AI.InSkillRange(6));
									node17.AddChild(new Mogo.AI.CastSpell(6,0));
									node17.AddChild(new Mogo.AI.EnterCD(500));
								}
								{
									Mogo.AI.SequenceNode node21 = new Mogo.AI.SequenceNode();
									node16.AddChild(node21);
									node21.AddChild(new Mogo.AI.ChooseCastPoint(6));
									node21.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
						{
							Mogo.AI.SequenceNode node24 = new Mogo.AI.SequenceNode();
							node10.AddChild(node24);
							node24.AddChild(new Mogo.AI.InSkillCoolDown(7));
							{
								Mogo.AI.Not node26 = new Mogo.AI.Not();
								node24.AddChild(node26);
								node26.Proxy(new Mogo.AI.InSkillRange(4));
							}
							{
								Mogo.AI.SelectorNode node28 = new Mogo.AI.SelectorNode();
								node24.AddChild(node28);
								{
									Mogo.AI.SequenceNode node29 = new Mogo.AI.SequenceNode();
									node28.AddChild(node29);
									node29.AddChild(new Mogo.AI.InSkillRange(7));
									node29.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,20));
									node29.AddChild(new Mogo.AI.CastSpell(7,0));
									node29.AddChild(new Mogo.AI.EnterCD(500));
								}
								{
									Mogo.AI.SequenceNode node34 = new Mogo.AI.SequenceNode();
									node28.AddChild(node34);
									node34.AddChild(new Mogo.AI.ChooseCastPoint(7));
									node34.AddChild(new Mogo.AI.MoveTo());
								}
							}
						}
						{
							Mogo.AI.SelectorNode node37 = new Mogo.AI.SelectorNode();
							node10.AddChild(node37);
							{
								Mogo.AI.SequenceNode node38 = new Mogo.AI.SequenceNode();
								node37.AddChild(node38);
								node38.AddChild(new Mogo.AI.InSkillCoolDown(5));
								node38.AddChild(new Mogo.AI.InSkillRange(5));
								node38.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
								node38.AddChild(new Mogo.AI.CastSpell(5,0));
								node38.AddChild(new Mogo.AI.EnterCD(500));
							}
							{
								Mogo.AI.SequenceNode node44 = new Mogo.AI.SequenceNode();
								node37.AddChild(node44);
								node44.AddChild(new Mogo.AI.InSkillCoolDown(4));
								node44.AddChild(new Mogo.AI.InSkillRange(4));
								node44.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
								node44.AddChild(new Mogo.AI.CastSpell(4,0));
								node44.AddChild(new Mogo.AI.EnterCD(500));
							}
							{
								Mogo.AI.SequenceNode node50 = new Mogo.AI.SequenceNode();
								node37.AddChild(node50);
								node50.AddChild(new Mogo.AI.InSkillCoolDown(3));
								node50.AddChild(new Mogo.AI.InSkillRange(3));
								node50.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
								node50.AddChild(new Mogo.AI.CastSpell(3,0));
								node50.AddChild(new Mogo.AI.EnterCD(500));
							}
							{
								Mogo.AI.SequenceNode node56 = new Mogo.AI.SequenceNode();
								node37.AddChild(node56);
								node56.AddChild(new Mogo.AI.InSkillCoolDown(1));
								node56.AddChild(new Mogo.AI.InSkillRange(1));
								node56.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
								node56.AddChild(new Mogo.AI.CastSpell(1,0));
								node56.AddChild(new Mogo.AI.EnterCD(500));
							}
							{
								Mogo.AI.SequenceNode node62 = new Mogo.AI.SequenceNode();
								node37.AddChild(node62);
								node62.AddChild(new Mogo.AI.InSkillCoolDown(2));
								node62.AddChild(new Mogo.AI.InSkillRange(2));
								node62.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
								node62.AddChild(new Mogo.AI.CastSpell(2,0));
								node62.AddChild(new Mogo.AI.EnterCD(500));
							}
							{
								Mogo.AI.SequenceNode node68 = new Mogo.AI.SequenceNode();
								node37.AddChild(node68);
								node68.AddChild(new Mogo.AI.ChooseCastPoint(1));
								node68.AddChild(new Mogo.AI.MoveTo());
							}
							node37.AddChild(new Mogo.AI.EnterRest(100));
						}
					}
				}
			}
		}
	}
}
