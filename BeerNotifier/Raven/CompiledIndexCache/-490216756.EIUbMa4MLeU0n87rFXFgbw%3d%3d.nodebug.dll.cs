using Raven.Abstractions;
using Raven.Database.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using Raven.Database.Linq.PrivateExtensions;
using Lucene.Net.Documents;
using System.Globalization;
using System.Text.RegularExpressions;
using Raven.Database.Indexing;


public class Index_Auto_2fParticipants_2fByForceAndLastPurchaseSortByLastPurchase : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fParticipants_2fByForceAndLastPurchaseSortByLastPurchase()
	{
		this.ViewText = @"from doc in docs.Participants
select new { LastPurchase = doc.LastPurchase, Force = doc.Force }";
		this.ForEntityNames.Add("Participants");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "Participants", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				LastPurchase = doc.LastPurchase,
				Force = doc.Force,
				__document_id = doc.__document_id
			});
		this.AddField("LastPurchase");
		this.AddField("Force");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("LastPurchase");
		this.AddQueryParameterForMap("Force");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("LastPurchase");
		this.AddQueryParameterForReduce("Force");
		this.AddQueryParameterForReduce("__document_id");
	}
}
