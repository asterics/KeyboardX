using NLog;
using Player.Model.Action;
using System;
using System.Speech.Synthesis;

namespace Player.Core.Action
{
    /// <summary>
    /// Speaks to the user via speech synthesis engine.
    /// </summary>
    /// TODOs:
    ///  TODO 1: if SpeechSynthesizer needs a lot of space or time to create, but is thread safe, create only one instance in whole application
    class TTSAction : BaseAction<TTSActionParameter>, IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private SpeechSynthesizer synthesizer;


        public TTSAction(ActionParameter param)
            : base(param)
        {
            synthesizer = new SpeechSynthesizer();
        }


        public override void DoAction()
        {
            synthesizer.Speak(Param.Message);
        }

        public void Dispose()
        {
            if (synthesizer != null)
                synthesizer.Dispose();
        }
    }
}
