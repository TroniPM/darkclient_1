namespace Mogo.AI.BT
{
	public sealed class BT10204 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT10204 _instance = null;
		public static BT10204 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT10204();

				return _instance;
			}
		}

		private BT10204()
		{
			{
				Mogo.AI.SelectorNode node1 = new Mogo.AI.SelectorNode();
				this.AddChild(node1);
				{
					Mogo.AI.SequenceNode node2 = new Mogo.AI.SequenceNode();
					node1.AddChild(node2);
					node2.AddChild(new Mogo.AI.AOI(0));
					{
						Mogo.AI.Not node4 = new Mogo.AI.Not();
						node2.AddChild(node4);
						node4.Proxy(new Mogo.AI.ISCD());
					}
					node2.AddChild(new Mogo.AI.IsTargetCanBeAttack());
					{
						Mogo.AI.SelectorNode node7 = new Mogo.AI.SelectorNode();
						node2.AddChild(node7);
						{
							Mogo.AI.SequenceNode node8 = new Mogo.AI.SequenceNode();
							node7.AddChild(node8);
							node8.AddChild(new Mogo.AI.InSkillCoolDown(7));
							node8.AddChild(new Mogo.AI.CmpTargetDistance(Mogo.AI.CmpType.lt,400));
							node8.AddChild(new Mogo.AI.CastSpell(7,1));
							node8.AddChild(new Mogo.AI.EnterCD(1000));
						}
						{
							Mogo.AI.SequenceNode node13 = new Mogo.AI.SequenceNode();
							node7.AddChild(node13);
							node13.AddChild(new Mogo.AI.InSkillCoolDown(5));
							node13.AddChild(new Mogo.AI.InSkillRange(5));
							node13.AddChild(new Mogo.AI.CastSpell(5,0));
							node13.AddChild(new Mogo.AI.EnterCD(1000));
						}
						{
							Mogo.AI.SequenceNode node18 = new Mogo.AI.SequenceNode();
							node7.AddChild(node18);
							node18.AddChild(new Mogo.AI.InSkillCoolDown(6));
							node18.AddChild(new Mogo.AI.InSkillRange(6));
							node18.AddChild(new Mogo.AI.CastSpell(6,0));
							node18.AddChild(new Mogo.AI.EnterCD(1000));
						}
						{
							Mogo.AI.SequenceNode node23 = new Mogo.AI.SequenceNode();
							node7.AddChild(node23);
							node23.AddChild(new Mogo.AI.InSkillCoolDown(1));
							node23.AddChild(new Mogo.AI.InSkillRange(1));
							node23.AddChild(new Mogo.AI.CastSpell(1,0));
							node23.AddChild(new Mogo.AI.EnterCD(0));
						}
						{
							Mogo.AI.SequenceNode node28 = new Mogo.AI.SequenceNode();
							node7.AddChild(node28);
							node28.AddChild(new Mogo.AI.ChooseCastPoint(1));
							node28.AddChild(new Mogo.AI.MoveTo());
						}
						node7.AddChild(new Mogo.AI.EnterRest(1000));
					}
				}
			}
		}
	}
}
