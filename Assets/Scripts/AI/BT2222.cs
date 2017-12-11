namespace Mogo.AI.BT
{
	public sealed class BT2222 : Mogo.AI.BehaviorTreeRoot
	{
		private static BT2222 _instance = null;
		public static BT2222 Instance
		{
			get
			{
				if(_instance == null)
					_instance = new BT2222();

				return _instance;
			}
		}

		private BT2222()
		{
			{
				Mogo.AI.SelectorNode node1 = new Mogo.AI.SelectorNode();
				this.AddChild(node1);
				{
					Mogo.AI.SequenceNode node2 = new Mogo.AI.SequenceNode();
					node1.AddChild(node2);
					node2.AddChild(new Mogo.AI.CastSpell(2,0));
				}
				{
					Mogo.AI.SequenceNode node4 = new Mogo.AI.SequenceNode();
					node1.AddChild(node4);
					node4.AddChild(new Mogo.AI.InSkillCoolDown(1));
					node4.AddChild(new Mogo.AI.CmpRate(Mogo.AI.CmpType.lt,80));
					node4.AddChild(new Mogo.AI.CastSpell(1,0));
				}
			}
		}
	}
}
