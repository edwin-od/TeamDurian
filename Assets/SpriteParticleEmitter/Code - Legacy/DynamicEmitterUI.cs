using System;
using UnityEngine;
using System.Collections.Generic;
using SpriteToParticlesAsset;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

namespace SpriteParticleEmitter
{
//[Obsolete("Use SpriteToParticles component instead")]
[ExecuteInEditMode]
[RequireComponent(typeof(UIParticleRenderer))]
/// <summary>
///Obsolete: Use SpriteToParticles component instead -  Refer to manual for description.
/// </summary>
public class DynamicEmitterUI : EmitterBaseUI
{
    [Tooltip("Start emitting as soon as able")]
    //! Start emitting as soon as able?
    public bool PlayOnAwake = true;

    [Header("Emission")]
    [Tooltip("Particles to emit per second")]
    //! Particles to emit per second
    public float EmissionRate = 1000;
    //! Save time to know how many particles to show per frame
    protected float ParticlesToEmitThisFrame;

    [Tooltip("Should the system cache sprites data? (Refer to manual for further explanation)")]
    //! Should the system cache sprites data? (Refer to manual for further explanation)
    public bool CacheSprites = true;

    //! Emiting from color needs to cycle all pixels in the sprite to know where the color is and later emition needs to randomize the emitting position so a look up table cache is needed
    //! Made it private but not local to the scope for reusing in next frames
    private Color[] colorCache = new Color[1];
    //! Emiting from color needs to cycle all pixels in the sprite to know where the color is and later emition needs to randomize the emitting position so a look up table cache is needed
    //! Made it private but not local to the scope for reusing in next frames
    private int[] indexCache = new int[1];

    //! Dictionary containing all sprites data so far for not asking texture.GetPixels() every frame, which would be slow.
    protected Dictionary<Sprite, Color[]> spritesSoFar = new Dictionary<Sprite, Color[]>();

    protected override void Awake()
    {
        base.Awake();
        if (PlayOnAwake)
            isPlaying = true;
        currentRectTransform = GetComponent<RectTransform>();
        targetRectTransform = imageRenderer.GetComponent<RectTransform>();

        #if UNITY_5_5_OR_NEWER
        if (mainModule.maxParticles < EmissionRate)
            mainModule.maxParticles = Mathf.CeilToInt(EmissionRate);
        #endif
    }

    //! The target Image Renderer's RectTransform
    private RectTransform targetRectTransform;
    //! This RectTransform
    private RectTransform currentRectTransform;

    protected Vector2 offsetXY;
    //! Multiplier used with texture's Pixels per unit
    protected float wMult = 100;
    //! Multiplier used with texture's Pixels per unit
    protected float hMult = 100;

    /// <summary>
    /// When playing it emits particles based on EmissionRate.
    /// </summary>
    protected void Update()
    {
        if (isPlaying)
        {
            if (imageRenderer == null)
            {
                if (verboseDebug)
                    Debug.LogError("Image Renderer component not referenced in DynamicEmitterUI component");
                isPlaying = false;
                return;
            }

            //match current RectTransform's data with target RectTransform
            if (matchImageRendererPostionData)
                currentRectTransform.position = new Vector3(targetRectTransform.position.x,
                    targetRectTransform.position.y, targetRectTransform.position.z);
            currentRectTransform.pivot = targetRectTransform.pivot;
            if (matchImageRendererPostionData)
            {
                currentRectTransform.anchoredPosition = targetRectTransform.anchoredPosition;
                currentRectTransform.anchorMin = targetRectTransform.anchorMin;
                currentRectTransform.anchorMax = targetRectTransform.anchorMax;
                currentRectTransform.offsetMin = targetRectTransform.offsetMin;
                currentRectTransform.offsetMax = targetRectTransform.offsetMax;
            }
            if (matchImageRendererScale)
                currentRectTransform.localScale = targetRectTransform.localScale;
            currentRectTransform.rotation = targetRectTransform.rotation;
            
            currentRectTransform.sizeDelta = new Vector2(targetRectTransform.rect.width, targetRectTransform.rect.height);

            //Calculate position multipliers based on pixels per unit
            float offsetX = (1-targetRectTransform.pivot.x) * (targetRectTransform.rect.width) - targetRectTransform.rect.width/2;
            float offsetY = (1 - targetRectTransform.pivot.y) * (-targetRectTransform.rect.height) + targetRectTransform.rect.height / 2;
            offsetXY = new Vector2(offsetX, offsetY);
            Sprite sprite = imageRenderer.sprite;

            if (!sprite)
            {
                if (verboseDebug)
                    Debug.LogError("Unable to get positions. Sprite is null in game object " + name);
                return;
            }

            //Zachary Scaling bug
            //// If the target image has "PreserveAspect" enabled then we need to figure out which dimension got shorter. 
            //if (imageRenderer.preserveAspect)
            //{
            //
            //    // Get the aspect ratio of the sprite here as defined by aspect = height / width. Keep this definition consistent for now. 
            //    // Remember that if the sprite aspect is larger than the target rect, the sprite was shrunken horizontally and the 'wMult' must be made smaller.
            //    // If the sprite aspect is smaller than the target rect, the sprite was shrunken vertically and the 'hMult' must be made smaller. 
            //    // This is based on the above definition that a = h/w. 
            //    float spriteAspect = sprite.rect.size.y / sprite.rect.size.x;
            //    float targetAspect = targetRectTransform.rect.height / targetRectTransform.rect.width;
            //
            //
            //    // If this sprite aspect is larger than the target RectTransform then we must scale the x-scale accordingly or the particles will be emitted too far out horizontally. 
            //    if (spriteAspect > targetAspect)
            //    {
            //        //Debug.Log("[DynamicEmitterUI] Sprite aspect larger - targetRectTransform aspect: " + targetAspect + ", sprite aspect: " + spriteAspect);
            //        wMult = sprite.pixelsPerUnit * (targetRectTransform.rect.width / sprite.rect.size.x) * (targetAspect / spriteAspect);
            //        hMult = sprite.pixelsPerUnit * (targetRectTransform.rect.height / sprite.rect.size.y);
            //    }
            //    else
            //    {
            //        //Debug.Log("[DynamicEmitterUI] targetRectTransform aspect larger - targetRectTransform aspect: " + targetAspect + ", sprite aspect: " + spriteAspect);
            //        wMult = sprite.pixelsPerUnit * (targetRectTransform.rect.width / sprite.rect.size.x);
            //        hMult = sprite.pixelsPerUnit * (targetRectTransform.rect.height / sprite.rect.size.y) * (spriteAspect / targetAspect);
            //    }
            //}
            //else
            //{
            //    wMult = sprite.pixelsPerUnit * (targetRectTransform.rect.width / sprite.rect.size.x);
            //    hMult = sprite.pixelsPerUnit * (targetRectTransform.rect.height / sprite.rect.size.y);
            //}

            wMult = sprite.pixelsPerUnit * (targetRectTransform.rect.width / sprite.rect.size.x);
            hMult = sprite.pixelsPerUnit * (targetRectTransform.rect.height / sprite.rect.size.y);

            ParticlesToEmitThisFrame += EmissionRate * Time.deltaTime;
            int EmissionCount = (int) ParticlesToEmitThisFrame;
            //don't even call the method if no particle would be emitted
            if (EmissionCount > 0)
                Emit(EmissionCount);
            ParticlesToEmitThisFrame -= EmissionCount;
        }
    }

    /// <summary>
    /// Randomly emit particles in sprite.
    /// </summary>
    /// <param name="emitCount">Number of particles to emit</param>
    public void Emit(int emitCount)
    {
        Sprite sprite = imageRenderer.sprite;
        if (imageRenderer.overrideSprite)
        {
            sprite = imageRenderer.overrideSprite;
            //Debug.Log(sprite.rect.size.x);
        }

        if (!sprite)
        {
            if (verboseDebug)
                Debug.LogError("Unable to emit. Sprite is null in game object " + name);
            return;
        }

        float colorR = EmitFromColor.r;
        float colorG = EmitFromColor.g;
        float colorB = EmitFromColor.b;

        float PixelsPerUnit = sprite.pixelsPerUnit;

        float width = (int) sprite.rect.size.x;
        float height = (int) sprite.rect.size.y;

        //set particle size based on sprite Pixels per unit and particle system prefered size
        #if UNITY_5_5_OR_NEWER
        ParticleSystem.MinMaxCurve startSize = mainModule.startSize;
        #else
            var startSize = particlesSystem.startSize;
        #endif

        //calculate sprite offset position in texture
        float offsetX = sprite.pivot.x / PixelsPerUnit;
        float offsetY = sprite.pivot.y / PixelsPerUnit;

        //if the sprite raw data is cached use that one, if not ask for it to the texture.
        Color[] pix;
        if (useSpritesSharingCache && Application.isPlaying)
        {
            pix = SpritesDataPool.GetSpriteColors(sprite, (int)sprite.rect.position.x, (int)sprite.rect.position.y, (int)width, (int)height);
        }
        else if (CacheSprites)
        {
            if (spritesSoFar.ContainsKey(sprite))
                pix = spritesSoFar[sprite];
            else
            {
                pix = sprite.texture.GetPixels((int) sprite.rect.position.x, (int) sprite.rect.position.y, (int) width, (int) height); 
                spritesSoFar.Add(sprite, pix);
            }
        }
        else
        {
            pix = sprite.texture.GetPixels((int) sprite.rect.position.x, (int) sprite.rect.position.y, (int) width, (int) height);
        }

        float toleranceR = RedTolerance;
        float toleranceG = GreenTolerance;
        float toleranceB = BlueTolerance;

        float widthByHeight = width*height;

        Color[] cCache = colorCache;
        int[] iCache = indexCache;

        if (cCache.Length < widthByHeight)
        {
            colorCache = new Color[(int) widthByHeight];
            indexCache = new int[(int) widthByHeight];
            cCache = colorCache;
            iCache = indexCache;
        }

        //TODO XXX this next "for" is the bottleneck for performance in big images. (optimization could be made, separating this for in 2 or more frames)
        //find available pixels to emit from
        int matchesCount = 0;
        for (int i = 0; i < widthByHeight; i++)
        {
            Color c = pix[i];
            //skip pixels with alpha 0
            if (c.a > 0)
            {
                //Skip unwanted colors when using Emission from color.
                if (UseEmissionFromColor) 
                    if(!FloatComparer.AreEqual(colorR, c.r, toleranceR) ||
                     !FloatComparer.AreEqual(colorG, c.g, toleranceG) ||
                     !FloatComparer.AreEqual(colorB, c.b, toleranceB))
                    continue;
                
                cCache[matchesCount] = c;
                iCache[matchesCount] = i;
                matchesCount++;
            }
        }
        //no colors were matched, stop
        if (matchesCount <= 0)
            return;

        Vector3 tempV = Vector3.zero;

        //emit needed particle count
        for (int k = 0; k < emitCount; k++)
        {
            int index = Random.Range(0, matchesCount - 1);
            int i = iCache[index];
            
            //get pixel position in texture
            float posX = ((i%width)/PixelsPerUnit) - offsetX;
            float posY = ((i/width)/PixelsPerUnit) - offsetY;
            
            ParticleSystem.EmitParams em = new ParticleSystem.EmitParams();
            // define new particle start position based on Sprite pixel position in texture, this game object's rotation and position.
            tempV.x = posX * wMult + offsetXY.x;
            tempV.y = posY * hMult - offsetXY.y;
            em.position = tempV;
            
            if (UsePixelSourceColor)
                em.startColor = cCache[index];

            #if UNITY_5_5_OR_NEWER
            em.startSize = startSize.constant;//TODO ability to process different sizes coming in next update
            #else
                em.startSize = startSize;
            #endif
            particlesSystem.Emit(em, 1);
        }
    }

    /// <summary>
    /// Will emit one particle from every pixel in the sprite, or from every pixel in the found color if UseEmissionFromColor is set to true
    /// </summary>
    /// <param name="hideSprite">Must it disable referenced spriteRenderer</param>
    public void EmitAll(bool hideSprite = true)
    {
        if (hideSprite)
            imageRenderer.enabled = false;

        Sprite sprite = imageRenderer.sprite;

        if (!sprite)
        {
            if (verboseDebug)
                Debug.LogError("Unable to emit. Sprite is null in game object " + name);
            return;
        }

        float colorR = EmitFromColor.r;
        float colorG = EmitFromColor.g;
        float colorB = EmitFromColor.b;

        float PixelsPerUnit = sprite.pixelsPerUnit;

        float width = (int) sprite.rect.size.x;
        float height = (int) sprite.rect.size.y;

        //set particle size based on sprite Pixels per unit and particle system prefered size
        #if UNITY_5_5_OR_NEWER
        var startSize = mainModule.startSize.constant; //TODO ability to process different sizes coming in next update
        #else
            var startSize = particlesSystem.startSize;
        #endif

        //calculate sprite offset position in texture
        float offsetX = sprite.pivot.x/PixelsPerUnit;
        float offsetY = sprite.pivot.y/PixelsPerUnit;

        //if the sprite raw data is cached use that one, if not ask for it to the texture.
        Color[] pix;
        if (useSpritesSharingCache && Application.isPlaying)
        {
            pix = SpritesDataPool.GetSpriteColors(sprite, (int)sprite.rect.position.x, (int)sprite.rect.position.y, (int)width, (int)height);
        }
        else if (CacheSprites)
        {
            if (spritesSoFar.ContainsKey(sprite))
                pix = spritesSoFar[sprite];
            else
            {
                pix = sprite.texture.GetPixels((int)sprite.rect.position.x, (int)sprite.rect.position.y, (int)width, (int)height);
                spritesSoFar.Add(sprite, pix);
            }
        }
        else
        {
            pix = sprite.texture.GetPixels((int)sprite.rect.position.x, (int)sprite.rect.position.y, (int)width, (int)height);
        }

        float toleranceR = RedTolerance;
        float toleranceG = GreenTolerance;
        float toleranceB = BlueTolerance;

        float widthByHeight = width*height;

        Vector3 tempV = Vector3.zero;

        for (int i = 0; i < widthByHeight; i++)
        {
            Color c = pix[i];
            //skip pixels with alpha 0
            if (c.a <= 0)
                continue;

            //Skip unwanted colors when using Emission from color.
            if (UseEmissionFromColor)
                if (!FloatComparer.AreEqual(colorR, c.r, toleranceR) ||
                 !FloatComparer.AreEqual(colorG, c.g, toleranceG) ||
                 !FloatComparer.AreEqual(colorB, c.b, toleranceB))
                    continue;

            //get pixel position in texture
            float posX = ((i % width) / PixelsPerUnit) - offsetX;
            float posY = ((i / width) / PixelsPerUnit) - offsetY;

            ParticleSystem.EmitParams em = new ParticleSystem.EmitParams();
            // define new particle start position based on Sprite pixel position in texture, this game object's rotation and position.
            tempV.x = posX * wMult + offsetXY.x;
            tempV.y = posY * hMult - offsetXY.y;
            em.position = tempV;

            if (UsePixelSourceColor)
                em.startColor = c;

            em.startSize = startSize;
            particlesSystem.Emit(em, 1);
        }
    }
#if UNITY_EDITOR
    private static List<DynamicEmitterUI> deUIs;

    void OnValidate()
    {
        //if (EditorApplication.isCompiling)
        //{
        //    if (deUIs == null)
        //    {
        //        deUIs = new List<DynamicEmitterUI>();
        //    }
        //    if (!deUIs.Contains(this) && uiParticleSystem != null && uiParticleSystem.enabled)
        //    {
        //        uiParticleSystem.enabled = false;
        //        deUIs.Add(this);hgjfd
        //    }
        //}
        if (particlesSystem)
            particlesSystem.Stop();
        isPlaying = false;
        Awake();
    }
    
    //[UnityEditor.Callbacks.DidReloadScripts]
    //private static void OnScriptsReloaded()
    //{
    //    if (deUIs != null)
    //    {
    //        foreach (DynamicEmitterUI particleRenderer in deUIs)
    //        {
    //            particleRenderer.uiParticleSystem.enabled = true;
    //        }
    //    }
    //    deUIs = null;
    //}

#endif

    /// <summary>
    /// Enable spriteRenderer if it was disabled.
    /// </summary>
    public void RestoreSprite()
    {
        if (imageRenderer)
            imageRenderer.enabled = true;
    }

    public override void Play()
    {
        if (!isPlaying && particlesSystem)
            particlesSystem.Play();
        isPlaying = true;
    }

    public override void Pause()
    {
        if (isPlaying && particlesSystem)
            particlesSystem.Pause();
        isPlaying = false;
    }

    public override void Stop()
    {
        isPlaying = false;
    }

    public override bool IsPlaying()
    {
        return isPlaying;
    }

    public override bool IsAvailableToPlay()
    {
        return true;
    }

    /// <summary>
    /// Clears the sprites cache
    /// </summary>
    public void ClearCachedSprites()
    {
        spritesSoFar = new Dictionary<Sprite, Color[]>();
    }
}
}