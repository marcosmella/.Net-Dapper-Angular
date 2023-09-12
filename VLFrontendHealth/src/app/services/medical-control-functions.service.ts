import { Observable } from "rxjs/internal/Observable";
import { Injectable } from "@angular/core";
import { map } from "rxjs/operators";

import { DialogData } from "../models/dialog-data.model";
import { MedicalControl, MedicalControlChild, MedicalControlGrid } from "../models/medical-control.model";
import { AbsenceService } from "./absence.service";
import { MedicalControlsService } from "./medical-controls.service";
import { ModalService } from "./modal.service";
import { AbsenceRequest } from "./../models/absence.model";

@Injectable({
	providedIn: "root"
})
export class MedicalControlFunctionsService {
	constructor(
		private medicalControlService: MedicalControlsService,
		private absenceService: AbsenceService,
		private modalService: ModalService
	) {}

	deleteAllTracking(tracking: Array<MedicalControlChild> | Array<MedicalControlGrid>): Promise<void> {
		return new Promise((resolve, reject) => {
			if (tracking.length > 0) {
				const runPromisesInSequence = async (iterable, action) => {
					for (const element of iterable) {
						await action(element.id, element.idAbsence)
							.then(() => {})
							.catch((error) => {
								reject(error);
							});
					}
					resolve();
				};

				runPromisesInSequence(tracking, this.deleteAbsenceAndMedicalControl.bind(this));
			} else {
				resolve();
			}
		});
	}

	delete(medicalControl: MedicalControl | MedicalControlChild): Promise<void> {
		if (this.isTracking(medicalControl)) {
			return this.deleteAbsenceAndMedicalControl(medicalControl.id, medicalControl.idAbsence);
		}
		return new Promise((resolve, reject) => {
			this.deleteAllTracking(medicalControl.tracking)
				.then(() => {
					this.deleteAbsenceAndMedicalControl(medicalControl.id, medicalControl.absence.id)
						.then(() => {
							resolve();
						})
						.catch((error) => {
							reject(error);
						});
				})
				.catch((error) => {
					reject(error);
				});
		});
	}

	isTracking(element: MedicalControl | MedicalControlChild): element is MedicalControlChild {
		return element._type === "MedicalControlChild";
	}

	deleteAbsenceAndMedicalControl(id: number, idAbsence: number): Promise<void> {
		return new Promise((resolve, reject) => {
			if (idAbsence) {
				this.deleteAbsence(idAbsence)
					.then(() => {
						this.deleteMedicalControl(id)
							.then(() => {
								resolve();
							})
							.catch((error) => {
								reject(error);
							});
					})
					.catch((error) => {
						reject(error);
					});
			} else {
				this.deleteMedicalControl(id)
					.then(() => {
						resolve();
					})
					.catch((error) => {
						reject(error);
					});
			}
		});
	}

	deleteMedicalControl(id: number): Promise<void> {
		return new Promise((resolve, reject) => {
			this.medicalControlService.delete(id).subscribe(
				() => {
					resolve();
				},
				(error) => {
					reject(error);
				}
			);
		});
	}

	cancelAbsence(id: number): Promise<void> {
		return new Promise((resolve, reject) => {
			this.absenceService.cancel(id).subscribe(
				() => {
					resolve();
				},
				(error) => {
					reject(error);
				}
			);
		});
	}

	deleteAbsence(id: number): Promise<void> {
		return new Promise((resolve, reject) => {
			this.absenceService.delete(id).subscribe(
				() => {
					resolve();
				},
				() => {
					this.cancelAbsence(id).then(
						() => {
							resolve();
						},
						(error) => {
							reject(error);
						}
					);
				}
			);
		});
	}

	openDialogDelete(hasAbsence: boolean, hasTracking: boolean, absenceWasProcessed?: boolean): Observable<any> {
		const data: DialogData = {
			title: "atention",
			message: "areYouSureForDelete",
			noButtonMessage: "cancel",
			okButtonMessage: "yesIAmSure"
		};

		if (hasAbsence && !absenceWasProcessed) {
			data.attentionToast = "medicalControlWithAbsenceDeleteMessage";
		}
		if (hasTracking) {
			data.warningToast = "medicalControlWithAbsenceAndTrackingDeleteMessage";
		}
		if (absenceWasProcessed) {
			data.attentionToast = "medicalControlWithAbsenceProcessDeleteMessage";
		}

		return this.modalService.openDialog(data);
	}

	hasProcessedAbsence(element: MedicalControl): Observable<boolean> {
		return this.absenceService.getById(element.absence.id).pipe(
			map((absence: AbsenceRequest) => {
				if (absence.processed) {
					return true;
				}
				if (element.tracking) {
					element.tracking.forEach((tracking) => {
						if (tracking.idAbsence > 0) {
							return this.hasProcessedAbsenceTracking(tracking).subscribe((isProcessed) => {
								if (isProcessed) {
									return true;
								}
							});
						}
					});
				}
				return false;
			})
		);
	}

	hasProcessedAbsenceTracking(tracking: MedicalControlChild): Observable<boolean> {
		return this.absenceService.getById(tracking.idAbsence).pipe(
			map((absence) => {
				if (absence.processed) {
					return true;
				}
				return false;
			})
		);
	}
}
