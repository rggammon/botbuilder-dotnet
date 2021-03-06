﻿using System.Collections.Generic;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.Sequence;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers
{
    public class IpEntityRecognizer : EntityRecognizer
    {
        public IpEntityRecognizer()
        {
        }

        protected override List<ModelResult> Recognize(string text, string culture)
        {
            return SequenceRecognizer.RecognizeIpAddress(text, culture);
        }
    }
}
