using System;
using System.Collections.Generic;
using System.Linq;

using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.API;

namespace EVEMon.Common
{
    public sealed class EveMailMessagesCollection : ReadonlyCollection<EveMailMessage>
    {
        private readonly CCPCharacter m_ccpCharacter;

        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal EveMailMessagesCollection(CCPCharacter ccpCharacter)
        {
            m_ccpCharacter = ccpCharacter;
        }
        
        #endregion
        
        
        #region Properties

        /// <summary>
        /// Gets or sets the number of new messages.
        /// </summary>
        /// <value>The new messages.</value>
        internal int NewMessages { get; private set; }
        
        #endregion


        #region Importation & Exportation

        /// <summary>
        /// Imports the eve mail messages IDs from a serializable object.
        /// </summary>
        /// <param name="eveMailMessagesIDs">The eve mail messages IDs.</param>
        internal void Import(string eveMailMessagesIDs)
        {
            if (String.IsNullOrEmpty(eveMailMessagesIDs))
                return;

            List<string> ids = eveMailMessagesIDs.Split(',').ToList();
            foreach (var id in ids)
            {
                m_items.Add(new EveMailMessage(m_ccpCharacter,
                                                new SerializableMailMessagesListItem()
                                                {
                                                    MessageID = long.Parse(id)
                                                }));
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable mail messages from the API.</param>
        internal void Import(IEnumerable<SerializableMailMessagesListItem> src)
        {
            NewMessages = 0;

            List<EveMailMessage> newMessages = new List<EveMailMessage>();

            // Import the mail messages from the API
            foreach (var srcEVEMailMessage in src)
            {
                // Is it an Inbox message ?
                if (m_ccpCharacter.CharacterID != srcEVEMailMessage.SenderID)
                {
                    // If it's a new mail message increase the counter
                    var message = m_items.FirstOrDefault(x => x.MessageID == srcEVEMailMessage.MessageID);
                    if (message == null)
                        NewMessages++;
                }

                newMessages.Add(new EveMailMessage(m_ccpCharacter, srcEVEMailMessage));
            }

            m_items.Clear();
            m_items.AddRange(newMessages);
        }

        /// <summary>
        /// Exports the eve mail messages IDs to a serializable object.
        /// </summary>
        /// <returns></returns>
        internal string Export()
        {
            List<string> serial = new List<string>();

            foreach (var message in m_items.Where(x => x.Sender != m_ccpCharacter.Name))
            {
                serial.Add(message.MessageID.ToString());
            }

            return string.Join(",", serial);
        }
        
        #endregion
    }
}