using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFigmaMCP.Editor
{
    [Serializable]
    public class FigmaLayoutPipelineProfile
    {
        [field:SerializeField] public string Id { get; private set; }
        [field:SerializeField] public string Description { get; private set; }

        [SerializeReference, SubclassSelector]
        private List<FigmaLayoutPipelineObjectStepBase> pipelineSteps = new()
        {
            new TextPipelineStep(),
            new RectTransformPipelineStep(),
            new ImagePipelineStep(),
            new VerticalGroupPipelineStep(),
            new HorizontalGroupPipelineStep(),
            new GridPipelineStep(),
            new ContentSizeFitterPipelineStep()
        };

        public List<FigmaLayoutPipelineObjectStepBase> PipelineSteps => pipelineSteps;
    }
}
