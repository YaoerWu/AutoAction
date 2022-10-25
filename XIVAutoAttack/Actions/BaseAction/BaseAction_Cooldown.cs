﻿using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using XIVAutoAttack.Combos;
using XIVAutoAttack.Helpers;
using XIVAutoAttack.Updaters;


namespace XIVAutoAttack.Actions.BaseAction
{
    internal partial class BaseAction
    {
        /// <summary>
        /// 这个技能已经运转了几个完整的GCD
        /// </summary>
        /// <param name="gcdCount">已经运转了多少个完整的GCD</param>
        /// <param name="abilityCount">已经运转了多少个能力技之后</param>
        /// <param name="addWeaponElapsed">是否要把<see cref="ActionUpdater.WeaponElapsed"/>加进去</param>
        /// <returns>是否已经冷却了这么久了</returns>
        internal bool ElapsedAfterGCD(uint gcdCount = 0, uint abilityCount = 0, bool addWeaponElapsed = true)
        {
            if (!IsCoolDown) return false;
            var elapsed = RecastTimeElapsed;
            return CooldownHelper.ElapsedAfterGCD(elapsed, gcdCount, abilityCount, addWeaponElapsed);
        }

        /// <summary>
        /// 这个技能已经进入冷却多少秒了
        /// </summary>
        /// <param name="gcdelapsed">已经进行了多少秒了</param>
        /// <param name="addWeaponElapsed">是否要把<see cref="ActionUpdater.WeaponElapsed"/>加进去</param>
        /// <returns>是否已经冷却了这么久了</returns>
        internal bool ElapsedAfter(float gcdelapsed, bool addWeaponElapsed = true)
        {
            if (!IsCoolDown) return false;
            var elapsed = RecastTimeElapsed;
            return CooldownHelper.ElapsedAfter(elapsed, gcdelapsed, addWeaponElapsed);
        }

        /// <summary>
        /// 距离下几个GCD转好这个技能能用吗。
        /// </summary>
        /// <param name="gcdCount">要隔着多少个完整的GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <param name="addWeaponRemain">是否要把<see cref="ActionUpdater.WeaponRemain"/>加进去</param>
        /// <returns>这个时间点是否起码有一层可以用</returns>
        internal bool WillHaveOneChargeGCD(uint gcdCount = 0, uint abilityCount = 0, bool addWeaponRemain = true)
        {
            if (HaveOneCharge) return true;
            var recast = RecastTimeOneCharge;
            return CooldownHelper.RecastAfterGCD(recast, gcdCount, abilityCount, addWeaponRemain);
        }

        /// <summary>
        /// 几秒钟以后能转好嘛
        /// </summary>
        /// <param name="remain">要多少秒呢</param>
        /// <param name="addWeaponRemain">是否要把<see cref="ActionUpdater.WeaponRemain"/>加进去</param>
        /// <returns>这个时间点是否起码有一层可以用</returns>
        internal bool WillHaveOneCharge(float remain, bool addWeaponRemain = true)
        {
            if (HaveOneCharge) return true;
            var recast = RecastTimeOneCharge;
            return CooldownHelper.RecastAfter(recast, remain, addWeaponRemain);
        }

        /// <summary>
        /// 复唱时间
        /// </summary>
        private unsafe float RecastTime => ActionManager.Instance()->GetRecastTime(ActionType.Spell, ID);
        /// <summary>
        /// 咏唱时间
        /// </summary>
        internal virtual int Cast100 => Action.Cast100ms - (Service.ClientState.LocalPlayer.HaveStatus(ObjectStatus.LightSpeed, ObjectStatus.Requiescat) ? 25 : 0);
        
        [Obsolete("能否尽量不用，然后用WillHaveOneCharge")]
        internal float RecastTimeRemain => RecastTime - RecastTimeElapsed;

        [Obsolete("能否尽量不用，然后用ElapsedAfter")]
        internal unsafe float RecastTimeElapsed => ActionManager.Instance()->GetRecastTimeElapsed(ActionType.Spell, ID);
        private  unsafe ushort MaxCharges => Math.Max(ActionManager.GetMaxCharges(ID, Service.ClientState.LocalPlayer.Level), (ushort)1);
        internal unsafe bool IsCoolDown => ActionManager.Instance()->IsRecastTimerActive(ActionType.Spell, ID);
        /// <summary>
        /// 是否起码有一层技能
        /// </summary>
        internal bool HaveOneCharge => IsCoolDown ? RecastTimeElapsed >= RecastTimeOneCharge : true;

        private float RecastTimeOneCharge => RecastTime / MaxCharges;

        /// <summary>
        /// 下一层转好的时间
        /// </summary>
        internal float RecastTimeRemainOneCharge => RecastTimeRemain % RecastTimeOneCharge;
    }
}