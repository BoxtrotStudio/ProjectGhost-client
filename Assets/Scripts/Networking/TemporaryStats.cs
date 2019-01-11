using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryStats {
    public static string SHOTS_FIRED = "shots_fired";
    public static string SHOTS_HIT = "shots_hit";
    public static string HAT_TRICK = "hat_trick";
    public static string WEAPON = "weapon";
    public static string ITEM_USAGE = "item";

    private Dictionary<string, long> stats = new Dictionary<string, long>();

    public TemporaryStats() { }

    public TemporaryStats Set(string stat, long value) {
        stats.Add(stat, value);
        return this;
    }

    public TemporaryStats PlusOne(string stat) {
        if (!stats.ContainsKey(stat))
            stats.Add(stat, 0L);

        stats.Add(stat, stats[stat] + 1);
        return this;
    }

    public TemporaryStats SubtractOne(string stat) {
        if (!stats.ContainsKey(stat))
            stats.Add(stat, 0L);

        stats.Add(stat, stats[stat] - 1);
        return this;
    }

    public long Get(string stat) {
        if (!stats.ContainsKey(stat))
            return 0;
        return stats[stat];
    }
}
