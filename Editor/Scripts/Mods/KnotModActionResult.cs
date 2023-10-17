using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knot.ProjectMod.Editor
{
    [Serializable]
    public class KnotModActionResult : IKnotModActionResult
    {
        public bool IsCompleted
        {
            get => _isCompleted;
            set => _isCompleted = value;
        }
        [SerializeField] private bool _isCompleted;

        public string ResultMessage
        {
            get => _resultMessage;
            set => _resultMessage = value;
        }
        [SerializeField] private string _resultMessage;


        public KnotModActionResult() { }

        public KnotModActionResult(bool isCompleted)
        {
            _isCompleted = isCompleted;
        }

        public KnotModActionResult(bool isCompleted, string resultMessage)
        {
            _isCompleted = isCompleted;
            _resultMessage = resultMessage;
        }


        public static KnotModActionResult Failed(string message = "") => new KnotModActionResult(false, message);

        public static KnotModActionResult Completed(string message = "") => new KnotModActionResult(true, message);

    }
}
