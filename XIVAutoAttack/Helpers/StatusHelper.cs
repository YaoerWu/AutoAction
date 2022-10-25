﻿using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;
using XIVAutoAttack.Actions;
using XIVAutoAttack.Combos.Melee;
using XIVAutoAttack.Updaters;


namespace XIVAutoAttack.Helpers
{
    internal static class StatusHelper
    {
        public static readonly SortedList<uint, EnemyLocation> ActionLocations = new SortedList<uint, EnemyLocation>()
        {
            {DRGCombo.Actions.FangandClaw.ID, EnemyLocation.Side },
            {DRGCombo.Actions.WheelingThrust.ID, EnemyLocation.Back },
            {DRGCombo.Actions.ChaosThrust.ID, EnemyLocation.Back },
            {25772, EnemyLocation.Back },
            {MNKCombo.Actions.Demolish.ID, EnemyLocation.Back },
            {MNKCombo.Actions.SnapPunch.ID, EnemyLocation.Side },
            {NINCombo.Actions.TrickAttack.ID, EnemyLocation.Back },
            {NINCombo.Actions.AeolianEdge.ID, EnemyLocation.Back },
            {NINCombo.Actions.ArmorCrush.ID, EnemyLocation.Side },
            {NINCombo.Actions.Suiton.ID, EnemyLocation.Back },
            {RPRCombo.Actions.Gibbet.ID, EnemyLocation.Side },
            {RPRCombo.Actions.Gallows.ID, EnemyLocation.Back },
            {SAMCombo.Actions.Gekko.ID, EnemyLocation.Back },
            {SAMCombo.Actions.Kasha.ID, EnemyLocation.Side },
        };


        /// <summary>
        /// 距离下几个GCD转好状态还在嘛。
        /// </summary>
        /// <param name="gcdCount">要隔着多少个完整的GCD</param>
        /// <param name="abilityCount">再多少个能力技之后</param>
        /// <param name="addWeaponRemain">是否要把<see cref="ActionUpdater.WeaponRemain"/>加进去</param>
        /// <returns>这个时间点这个状态还健在嘛</returns>
        internal static bool WillStatusEndGCD(this BattleChara obj, uint gcdCount = 0, uint abilityCount = 0, bool addWeaponRemain = true, params ushort[] effectIDs)
        {
            var remain = obj.FindStatusTime(effectIDs);
            if (remain == 0) return false;
            return CooldownHelper.RecastAfterGCD(remain, gcdCount, abilityCount, addWeaponRemain);
        }

        /// <summary>
        /// 距离下几个GCD转好这个技能能用吗。
        /// </summary>
        /// <param name="remain">要多少秒呢</param>
        /// <param name="addWeaponRemain">是否要把<see cref="ActionUpdater.WeaponRemain"/>加进去</param>
        /// <returns>这个时间点这个状态还健在嘛</returns>
        internal static bool WillStatusEnd(this BattleChara obj, float remainWant, bool addWeaponRemain = true, params ushort[] effectIDs)
        {
            var remain = obj.FindStatusTime(effectIDs);
            if (remain == 0) return false;
            return CooldownHelper.RecastAfter(remain, remainWant, addWeaponRemain);
        }

        [Obsolete("计算时间的话，用用WillStatusEnd？")]
        internal static float FindStatusTime(this BattleChara obj, params ushort[] effectIDs)
        {
            var times = obj.FindStatusTimes(effectIDs);
            if (times == null || times.Length == 0) return 0;
            return times.Max();
        }

        [Obsolete("计算时间的话，用用WillStatusEnd？")]
        internal static float[] FindStatusTimes(this BattleChara obj, params ushort[] effectIDs)
        {
            return obj.FindStatus(effectIDs).Select(status => status.RemainingTime).ToArray();
        }

        internal static byte FindStatusStack(this BattleChara obj, params ushort[] effectIDs)
        {
            var stacks = obj.FindStatusStacks(effectIDs);
            if (stacks == null || stacks.Length == 0) return 0;
            return stacks.Max();
        }

        internal static byte[] FindStatusStacks(this BattleChara obj, params ushort[] effectIDs)
        {
            return obj.FindStatus(effectIDs).Select(status => Math.Max(status.StackCount, (byte)1)).ToArray();
        }

        internal static bool HaveStatus(this BattleChara obj, params ushort[] effectIDs)
        {
            return obj.FindStatus(effectIDs).Length > 0;
        }

        internal static Status[] FindStatus(this BattleChara obj, params ushort[] effectIDs)
        {
            uint[] newEffects = effectIDs.Select(a => (uint)a).ToArray();
            return obj.FindAllStatus().Where(status => newEffects.Contains(status.StatusId)).ToArray();
        }

        private static Status[] FindAllStatus(this BattleChara obj)
        {
            if (obj == null) return new Status[0];

            return obj.StatusList.Where(status => status.SourceID == Service.ClientState.LocalPlayer.ObjectId).ToArray();
        }
    }
}