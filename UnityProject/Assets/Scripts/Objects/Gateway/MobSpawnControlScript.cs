﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MobSpawnControlScript : NetworkBehaviour
{
	public List<MobSpawnScript> MobSpawners = new List<MobSpawnScript>();

	public bool DetectViaMatrix;

	private bool SpawnedMobs;

	private const float PlayerCheckTime = 1f;

	[Server]
	public void SpawnMobs()
	{
		if (SpawnedMobs) return;
		SpawnedMobs = true;

		foreach (var Spawner in MobSpawners)
		{
			if (Spawner != null)
			{
				Spawner.SpawnMob();
			}
		}
	}

	[ContextMenu("Rebuild mob spawner list")]
	void RebuildMobSpawnerList()
	{
		MobSpawners.Clear();
		foreach (Transform t in transform.parent)
		{
			var mobSpawner = t.GetComponent<MobSpawnScript>();
			if (mobSpawner != null)
			{
				MobSpawners.Add(mobSpawner);
			}
		}
		#if UNITY_EDITOR
		EditorUtility.SetDirty(gameObject);
		#endif
	}

	protected virtual void UpdateMe()
	{
		DetectPlayer();
	}

	private void OnEnable()
	{
		if (!DetectViaMatrix) return;
		if (!CustomNetworkManager.IsServer) return;

		UpdateManager.Add(UpdateMe, PlayerCheckTime);
	}

	void OnDisable()
	{
		if (!CustomNetworkManager.IsServer) return;

		UpdateManager.Remove(CallbackType.PERIODIC_UPDATE, UpdateMe);
	}

	[Server]
	private void DetectPlayer()
	{
		foreach (var player in PlayerList.Instance.InGamePlayers)
		{
			var script = player.Script;
			if (script == null) return;

			if (script.IsNormal && script.registerTile.Matrix == gameObject.GetComponent<RegisterObject>().Matrix)
			{
				SpawnMobs();
				UpdateManager.Remove(CallbackType.PERIODIC_UPDATE, UpdateMe);
				return;
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		var sprite = GetComponentInChildren<SpriteRenderer>();
		if (sprite == null)
			return;

		//Highlighting all controlled lightSources
		Gizmos.color = new Color(0.5f, 0.5f, 1, 1);
		for (int i = 0; i < MobSpawners.Count; i++)
		{
			var mobSpawner = MobSpawners[i];
			if (mobSpawner == null) continue;
			Gizmos.DrawLine(sprite.transform.position, mobSpawner.transform.position);
			Gizmos.DrawSphere(mobSpawner.transform.position, 0.25f);
		}
	}
}
