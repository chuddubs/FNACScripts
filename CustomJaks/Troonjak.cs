using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Troonjak : Jak
{
    public GameObject withChud;
    public GameObject hecking;
    public GameObject valid;
    private Coroutine ack;
    private bool ackCountdownLaunched = false;
    private int flashesToBacktrack = 3;
    public AudioClip    flyBuzzing;
    public AudioClip    ackSound;
    private Jak         chud;
    private Jak         rapeson;
    public KeyboardEasterEgg egg;

    private void Start()
    {
        chud = NightManager.Instance.jaks.First(jak => jak.variant == Variant.Chud);
        rapeson = NightManager.Instance.jaks.First(j => j.variant == Variant.Rapeson);

    }

    private void OnEnable()
    {
        OfficeController.Instance.onFlashLightToggled += OnFlashLightToggled;
    }

    private void OnDestroy()
    {
        if(!this.gameObject.scene.isLoaded)
            return;    
        OfficeController.Instance.onFlashLightToggled -= OnFlashLightToggled;
    }

    public override void StartNight(int n)
    {
        base.StartNight(n);
        flashesToBacktrack = 3;
        egg.enabled = IsActiveTonight();
    }

    private IEnumerator PrepareAck()
    {
        ackCountdownLaunched = true;
        audioSource.Play();
        FeralManager.Instance.Disturb(15f);
        flashesToBacktrack = 3;
        float timer = Random.Range(5f, 7f);
        while (timer > 0)
        {
            if (flashesToBacktrack == 2)
                ChangeSprite(hecking);
            if (flashesToBacktrack == 1)
                ChangeSprite(valid);
            if (flashesToBacktrack <= 0)
            {
                audioSource.Stop();
                // Debug.Log("You just saved a trans life");
                if (!OfficeController.Instance.doorOpen)
                {
                    OfficeController.Instance.OpenCloseDoor(true);
                    yield return new WaitForSeconds(0.6f);
                }
                Teleport(currentBehavior.GetRandomStartPos());
                flashesToBacktrack = 3;
                AchievementsManager.Instance.UpdateChievoStat("chievo7", "TroonExp");
                ackCountdownLaunched = false;
                yield break;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        Ackkk(false);
        ackCountdownLaunched = false;
    }

    public void Ackkk(bool fromEgg = true)
    {
        if (fromEgg)
            AchievementsManager.Instance.UnlockChievo("secretchievo2");
        StopAck();
        ShowHide(false);
        bool caca = SoyGameController.Instance.IsCaca();
        if (!caca)
        {
            AudioManager.Instance.Play(ackSound);
            AchievementsManager.Instance.OnTroonAck();
        }
        else
            AudioManager.Instance.PlayStory();
        FeralManager.Instance.Disturb(12f);
        OfficeController.Instance.ACK.SetActive(true);
        EndNight();
    }

    private void StopAck()
    {
        if (ack != null)
        {
            StopCoroutine(ack);
            ack = null;
        }
        ackCountdownLaunched = false;
    }

    public override void UpdateVisibility()
    {
		if (currentSpot.Cam.IsHallway)
            ShowHide(OfficeController.Instance.flashlight.activeSelf);
        else if (IsInOffice)
            ShowHide(!OfficeController.Instance.InDarkness);
        else
            ShowHide(IsActiveTonight());
    }

    private void OnFlashLightToggled(bool on)
    {
        if (IsInFlashlight() && IsInOffice)
            flashesToBacktrack--;
    }

    public override void UpdateJak()
    {
        if (!active)
            return;
        if (IsInOffice)
        {
            if (!ackCountdownLaunched)
                ack = StartCoroutine(PrepareAck());
        }
        else if (inCooldown && !Locked())
        {
            if (cooldown <= 0)
            {
                if (currentSpot.Cam.id == -1)
                {
                    if (OfficeController.Instance.doorOpen && !rapeson.IsInOffice)
                        StartCoroutine(Move());
                }
                else
                    AttemptMove();
            }
            else
                cooldown -= Time.deltaTime;
        }
        if (currentSpot.Cam.id == -1)
            ChangeSprite(inOffice);
        if (currentSpot.Cam.id == 7)
            ChangeSprite(chud.CurrentSpot.Cam.id == 7 ? withChud : standingNear);
    }

    public override void Interrupt()
    {
        base.Interrupt();
        StopAck();
        flashesToBacktrack = 3;
    }
}
