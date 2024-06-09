﻿using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using ProtoBuf.Meta;
using SteamKit2.GC.CSGO.Internal;

namespace NetHookAnalyzer2.Specializations
{
    class CSGOSOSingleObjectGCSpecialization : IGameCoordinatorSpecialization
    {
        public IEnumerable<KeyValuePair<string, object>> GetExtraObjects(object body, uint appID)
        {
            if (appID != WellKnownAppIDs.CounterStrike2)
            {
                yield break;
            }

            var updateSingle = body as CMsgSOSingleObject;
            if (updateSingle == null)
            {
                yield break;
            }

            var extraNode = ReadExtraObject(updateSingle);
            if (extraNode != null)
            {
                yield return new KeyValuePair<string, object>(string.Format("SO ({0})", extraNode.GetType().Name), extraNode);
            }
        }

        object ReadExtraObject(CMsgSOSingleObject sharedObject)
        {
            try
            {
                using var ms = new MemoryStream( sharedObject.object_data );
                if ( CSGOSOHelper.SOTypes.TryGetValue( sharedObject.type_id, out var t ) )
                {
                    return RuntimeTypeModel.Default.Deserialize( ms, null, t );
                }
            }
            catch (ProtoException ex)
            {
                return "Error parsing SO data: " + ex.Message;
            }
            catch (EndOfStreamException ex)
            {
                return "Error parsing SO data: " + ex.Message;
            }

            return null;
        }
    }
}
