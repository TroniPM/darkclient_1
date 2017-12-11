namespace Mogo.AI.BT
{
	public sealed class BT1024 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT1024 _instance = null;
		public static BT1024 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT1024();

				return _instance;
			}
		}

		private BT1024()
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
							node11.AddChild(new Mogo.AI.InSkillCoolDown(4));
							node11.AddChild(new Mogo.AI.InSkillRange(4));
							node11.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,25));
							node11.AddChild(new Mogo.AI.CastSpell(4,0));
							node11.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node17 = new Mogo.AI.SequenceNode();
							node10.AddChild(node17);
							node17.AddChild(new Mogo.AI.InSkillCoolDown(3));
							node17.AddChild(new Mogo.AI.InSkillRange(3));
							node17.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,30));
							node17.AddChild(new Mogo.AI.CastSpell(3,0));
							node17.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node23 = new Mogo.AI.SequenceNode();
							node10.AddChild(node23);
							node23.AddChild(new Mogo.AI.InSkillCoolDown(2));
							node23.AddChild(new Mogo.AI.InSkillRange(2));
							node23.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,40));
							node23.AddChild(new Mogo.AI.CastSpell(2,0));
							node23.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node29 = new Mogo.AI.SequenceNode();
							node10.AddChild(node29);
							node29.AddChild(new Mogo.AI.InSkillCoolDown(1));
							node29.AddChild(new Mogo.AI.InSkillRange(1));
							node29.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,50));
							node29.AddChild(new Mogo.AI.CastSpell(1,0));
							node29.AddChild(new Mogo.AI.EnterCD(0));
						}
						node10.AddChild(new Mogo.AI.LookOn(600,150,1000,25,1500,10,1000,5,1000,5,1000,55,0,1));
					}
				}
			}
		}
	}
}
