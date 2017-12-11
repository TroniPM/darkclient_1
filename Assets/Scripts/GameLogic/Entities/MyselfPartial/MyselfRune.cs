using System;
using System.Collections.Generic;

namespace Mogo.Game
{
	public partial class EntityMyself
    {
        private void AddRuneToBagReq(string uuid, byte idx, UInt32 resID, UInt32 exp)
        {
            //string uuid = (string)args[0];
            //int idx = (int)(byte)args[1];
            //int resID = (int)(UInt32)args[2];
            //int exp = (int)(UInt32)args[3];
            runeManager.AddRuneToBag(uuid, (int)idx, (int)resID, (int)exp);

        }

        private void DelRuneFromBagReq(byte idx)
        {
            //int idx = (int)(byte)args[0];
            runeManager.DelRuneFromBag((int)idx);
        }

        private void PutOnRuneResp(byte idx, byte posi)
        {
            //int idx = (int)(byte)args[0];
            //int posi = (int)(byte)args[1];
            runeManager.PutOnRuneResp((int)idx, (int)posi);
        }

        private void PutDownRuneResp(byte posi, byte idx)
        {
            //int posi = (int)(byte)args[0];
            //int idx = (int)(byte)args[1];
            runeManager.PutDownRuneResp((int)posi, (int)idx);
        }

        private void AddRuneToBodyReq(string uuid, byte posi, UInt32 resID, UInt32 exp)
        {
            //string uuid = (string)args[0];
            //int posi = (int)(byte)args[1];
            //int resID = (int)(UInt32)args[2];
            //int exp = (int)(UInt32)args[3];
            runeManager.AddRuneToBody(uuid, (int)posi, (int)resID, (int)exp);
        }

        private void DelRuneFromBodyReq(byte posi)
        {
            //int posi = (int)(byte)args[0];
            runeManager.DelRuneFromBody((int)posi);
        }

        private void UpdatePosiLockedReq(byte posi, byte locked)
        {
            //int _posi = (int)(byte)args[0];
            //bool _locked = (bool)args[1];
            runeManager.UpdatePosition((int)posi, ((int)locked == -1));
        }

        private void UpdateRefreshPriceReq(UInt32 gameMoney, UInt32 rmb)
        {
            //int _gameMoney = (int)(UInt32)args[0];
            //int _rmb = (int)(UInt32)args[1];
            runeManager.UpdateRefreshPrice((int)gameMoney, (int)rmb);
        }
    }
}
