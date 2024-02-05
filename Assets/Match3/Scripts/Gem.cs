﻿using UnityEngine;

namespace IndiMatchThree
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Gem:MonoBehaviour
    {
        public GemType type;

        public void SetType(GemType type)
        {
            this.type = type;
            GetComponent<SpriteRenderer>().sprite = type.sprite;
        }

        public GemType GetType() => type;

        public void DestroyGem()=>Destroy(gameObject);
    }
}