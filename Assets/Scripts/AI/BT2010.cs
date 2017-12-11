namespace Mogo.AI.BT
{
	public sealed class BT2010 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT2010 _instance = null;
		public static BT2010 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT2010();

				return _instance;
			}
		}

		private BT2010()
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
							{
								Mogo.AI.Not node12 = new Mogo.AI.Not();
								node11.AddChild(node12);
								node12.Proxy(new Mogo.AI.InSkillRange(1));
							}
							{
								Mogo.AI.SelectorNode node14 = new Mogo.AI.SelectorNode();
								node11.AddChild(node14);
								{
									Mogo.AI.SequenceNode node15 = new Mogo.AI.SequenceNode();
									node14.AddChild(node15);
									node15.AddChild(new Mogo.AI.InSkillRange(3));
									node15.AddChild(new Mogo.AI.InSkillCoolDown(3));
									node15.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
									node15.AddChild(new Mogo.AI.CastSpell(3,0));
									node15.AddChild(new Mogo.AI.EnterCD(500));
								}
								{
									Mogo.AI.SequenceNode node21 = new Mogo.AI.SequenceNode();
									node14.AddChild(node21);
									node21.AddChild(new Mogo.AI.InSkillRange(5));
									node21.AddChild(new Mogo.AI.InSkillCoolDown(5));
									node21.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,20));
									node21.AddChild(new Mogo.AI.CastSpell(5,0));
									node21.AddChild(new Mogo.AI.EnterCD(500));
								}
								{
									Mogo.AI.SequenceNode node27 = new Mogo.AI.SequenceNode();
									node14.AddChild(node27);
									node27.AddChild(new Mogo.AI.InSkillRange(6));
									node27.AddChild(new Mogo.AI.InSkillCoolDown(6));
									node27.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
									node27.AddChild(new Mogo.AI.CastSpell(6,0));
									node27.AddChild(new Mogo.AI.EnterCD(500));
								}
							}
						}
						{
							Mogo.AI.SequenceNode node33 = new Mogo.AI.SequenceNode();
							node10.AddChild(node33);
							node33.AddChild(new Mogo.AI.InSkillRange(1));
							{
								Mogo.AI.SelectorNode node35 = new Mogo.AI.SelectorNode();
								node33.AddChild(node35);
								{
									Mogo.AI.SequenceNode node36 = new Mogo.AI.SequenceNode();
									node35.AddChild(node36);
									node36.AddChild(new Mogo.AI.InSkillCoolDown(1));
									node36.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
									node36.AddChild(new Mogo.AI.CastSpell(1,0));
									node36.AddChild(new Mogo.AI.EnterCD(500));
								}
								{
									Mogo.AI.SequenceNode node41 = new Mogo.AI.SequenceNode();
									node35.AddChild(node41);
									node41.AddChild(new Mogo.AI.InSkillCoolDown(4));
									node41.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
									node41.AddChild(new Mogo.AI.CastSpell(4,0));
									node41.AddChild(new Mogo.AI.EnterCD(500));
								}
								{
									Mogo.AI.SequenceNode node46 = new Mogo.AI.SequenceNode();
									node35.AddChild(node46);
									node46.AddChild(new Mogo.AI.InSkillRange(2));
									node46.AddChild(new Mogo.AI.InSkillCoolDown(2));
									node46.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
									node46.AddChild(new Mogo.AI.CastSpell(2,0));
									node46.AddChild(new Mogo.AI.EnterCD(500));
								}
							}
						}
						{
							Mogo.AI.SequenceNode node52 = new Mogo.AI.SequenceNode();
							node10.AddChild(node52);
							node52.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,80));
							node52.AddChild(new Mogo.AI.ChooseCastPoint(1));
							node52.AddChild(new Mogo.AI.MoveTo());
						}
						node10.AddChild(new Mogo.AI.EnterRest(500));
					}
				}
			}
		}
	}
}
