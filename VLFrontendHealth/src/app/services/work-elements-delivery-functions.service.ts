import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";
import { map } from "rxjs/operators";

import {
	WorkElementDelivery,
	WorkElementDeliveryRow,
	WorkElementCategory,
	WorkElementDeliveryStatus
} from "../models/employee-work-element.model";
import { TranslatePipe } from "../pipes/translate.pipe";
import { WorkElementsDeliveryService } from "./work-elements-delivery.service";

@Injectable({
	providedIn: "root"
})
export class WorkElementsDeliveryFunctionsService {
	constructor(private workElementDeliveryService: WorkElementsDeliveryService, private translatePipe: TranslatePipe) {}

	getWorkElementDeliveryByEmployee(idEmployee: number): Observable<WorkElementDeliveryRow[]> {
		return this.workElementDeliveryService
			.getWorkElementsDeliveryByEmployee(idEmployee)
			.pipe(map((deliveries) => this.mapDeliveriesToGrid(deliveries)));
	}

	private getElementStatus(idTuRecibo: number): WorkElementDeliveryStatus {
		return idTuRecibo ? WorkElementDeliveryStatus.sended : WorkElementDeliveryStatus.notSended;
	}

	private mapDeliveriesToGrid(deliveries: WorkElementDelivery[]): WorkElementDeliveryRow[] {
		const elementDelivery: WorkElementDeliveryRow[] = [];
		deliveries.forEach((delivery) =>
			delivery.workElements.forEach((detail, index) => {
				if (detail.element.idCategory === WorkElementCategory.protection) {
					elementDelivery.push({
						id: index,
						date: delivery.date,
						idDelivery: delivery.id,
						idElement: detail.idEmployee,
						categoryDescription: this.translatePipe.transform(WorkElementCategory[detail.element.idCategory]),
						elementDescription: detail.element.description,
						amount: detail.amount,
						status: this.getElementStatus(delivery.idTuRecibo),
						option: detail.option,
						signed: delivery.signedConfirmationReceived,
						claim: detail.element.claim,
						idDetail: detail.element.idDetail,
						idFile: delivery.idFile
					});
				}
			})
		);
		return elementDelivery;
	}
}
