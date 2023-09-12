import { Component, ViewChild, AfterViewInit, OnInit, OnDestroy } from "@angular/core";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import { ActivatedRoute, Router } from "@angular/router";
import * as moment from "moment";
import { first } from "rxjs/operators";

import { LoadingSpinnerComponent } from "../../loading-spinner/loading-spinner.component";
import { MedicalControl, MedicalControlChild } from "./../../../models/medical-control.model";
import { MedicalControlTypeService } from "./../../../services/medical-control-type.service";
import { MedicalControlActionService } from "./../../../services/medical-control-action.service";
import { Select } from "./../../../models/select.model";
import { ElipsisAction, IconTypes } from "../../elipsis-grid/elipsis-grid.component";
import { MedicalControlsService } from "./../../../services/medical-controls.service";
import { AbsenceRequest } from "./../../../models/absence.model";
import { AbsenceService } from "./../../../services/absence.service";
import { AlertService } from "./../../../services/alert.service";
import { UserPreference } from "./../../../models/user-preference.model";
import { SnackBarService } from "../../../services/snack-bar.service";
import { MedicalControlTrackingTypesService } from "./../../../services/medical-control-tracking-types.service";
import { MedicalControlFunctionsService } from "./../../../services/medical-control-functions.service";
import { MenuService } from "../../../services/menu.service";

@Component({
	selector: "app-medical-control-detail",
	templateUrl: "./medical-control-detail.component.html",
	styleUrls: ["./medical-control-detail.component.scss"]
})
export class MedicalControlDetailComponent implements OnInit, AfterViewInit, OnDestroy {
	@ViewChild(MatSort, { static: true }) sort: MatSort;
	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild("spinner", { static: false })
	public spinner: LoadingSpinnerComponent;
	public medicalControlDisplayedColumns: string[] = [
		"controlDate",
		"controlTypeDescription",
		"controlActionTypeDescription",
		"controlRestDays",
		"options"
	];
	public trackingDisplayedColumns: string[] = [
		"trackingDate",
		"trackingControlTypeDescription",
		"trackingDescription",
		"trackingRestDays",
		"options"
	];
	public medicalControlDataSource: MatTableDataSource<MedicalControl>;
	public medicalControlTrackingDataSource: MatTableDataSource<MedicalControlChild>;
	public controlTypes: Array<Select>;
	public actionTypes: Array<Select>;
	public actionsMedicalControl: ElipsisAction[];
	public actionsTracking: ElipsisAction[];
	public idMedicalControl: number;
	public trackingType: Array<Select>;
	public isDeleteTracking: number;

	constructor(
		private router: Router,
		private medicalControlTypesService: MedicalControlTypeService,
		private medicalControlActionTypesService: MedicalControlActionService,
		private route: ActivatedRoute,
		private medicalControlService: MedicalControlsService,
		private absenceService: AbsenceService,
		private medicalControlFunctionService: MedicalControlFunctionsService,
		private alertService: AlertService,
		private snackBarService: SnackBarService,
		private medicalControlTrackingTypeService: MedicalControlTrackingTypesService,
		public userPreference: UserPreference,
		private menuService: MenuService
	) {
		this.medicalControlDataSource = new MatTableDataSource<MedicalControl>();
		this.medicalControlTrackingDataSource = new MatTableDataSource<MedicalControlChild>();
	}

	ngOnInit(): void {
		this.menuService.hideMenu();
	}

	ngAfterViewInit(): void {
		this.route.paramMap.pipe(first()).subscribe((params) => {
			this.idMedicalControl = Number(params.get("id"));
			if (this.idMedicalControl) {
				this.getMedicalControlPromises(this.idMedicalControl).then();
			}
		});
		this.medicalControlElipsis();
		this.medicalControlTracking();
	}

	ngOnDestroy(): void {
		this.menuService.showMenu();
	}

	getMedicalControlPromises(idMedicalControl: number): Promise<any> {
		return new Promise((resolve) => {
			const promises: Array<Promise<any>> = [];
			promises.push(
				this.getControlTypes().then((types) => {
					this.controlTypes = types;
				}),
				this.getActionTypes().then((actions) => {
					this.actionTypes = actions;
				}),
				this.getTrackingTypes().then((actions) => {
					this.trackingType = actions;
				})
			);
			Promise.all(promises).then(() => {
				resolve(this.getMedicalControl(idMedicalControl));
			});
		});
	}

	medicalControlElipsis(): void {
		this.actionsMedicalControl = new Array<ElipsisAction>();

		this.actionsMedicalControl.push(
			{
				action: () => this.editMedicalControl(),
				icon: IconTypes.edit
			},
			{
				action: (element) => this.elipsisDelete(element, true),
				icon: IconTypes.delete
			}
		);
	}

	medicalControlTracking(): void {
		this.actionsTracking = new Array<ElipsisAction>();

		this.actionsTracking.push(
			{
				action: (element) => this.editTracking(element.id),
				icon: IconTypes.edit
			},
			{
				action: (element) => this.elipsisDelete(element, false),
				icon: IconTypes.delete,
				condition: (element) => {
					return element.id === this.isDeleteTracking;
				}
			}
		);
	}

	editMedicalControl(): void {
		this.router.navigateByUrl(`/healthApp/medical-control/${this.idMedicalControl}`);
	}

	dblclickMedicalControl(id: number): void {
		this.idMedicalControl = id;
		this.editMedicalControl();
	}

	editTracking(id: number): void {
		const params: any = { parent: this.idMedicalControl };
		this.router.navigate(["healthApp", "medical-control", `${id}`], { queryParams: params });
	}

	dblclickTracking(id: number): void {
		this.editTracking(id);
	}

	elipsisDelete(element: MedicalControl | MedicalControlChild, isMedicalControl: boolean): void {
		if (isMedicalControl) {
			const medicalControl: MedicalControl = <MedicalControl>element;
			this.deleteMedicalControl(medicalControl, isMedicalControl);
		} else {
			const medicalControlTracking: MedicalControlChild = <MedicalControlChild>element;
			this.deleteMedicalControlTracking(medicalControlTracking, isMedicalControl);
		}
	}

	deleteMedicalControl(medicalControl: MedicalControl, isMedicalControl: boolean): void {
		let hasAbsence: boolean;
		let hasTracking: boolean;
		const absenceHasProcessed = false;
		hasAbsence = medicalControl.absence.id > 0;
		hasTracking = medicalControl.tracking.length > 0;
		if (medicalControl.absence?.id > 0) {
			this.medicalControlFunctionService.hasProcessedAbsence(medicalControl).subscribe(
				(hasAbsenceProcessed: boolean) => {
					this.openDialogDelete(medicalControl, isMedicalControl, hasAbsence, hasTracking, hasAbsenceProcessed);
				},
				(error) => {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			);
		} else {
			this.openDialogDelete(medicalControl, isMedicalControl, hasAbsence, hasTracking, absenceHasProcessed);
		}
	}

	deleteMedicalControlTracking(medicalControlTracking: MedicalControlChild, isMedicalControl: boolean): void {
		let hasAbsence: boolean;
		let hasTracking: boolean;
		hasAbsence = medicalControlTracking.idAbsence > 0;
		hasTracking = false;
		if (medicalControlTracking.idAbsence > 0) {
			this.medicalControlFunctionService.hasProcessedAbsenceTracking(medicalControlTracking).subscribe(
				(absenceWasProcessed: boolean) => {
					this.openDialogDelete(medicalControlTracking, isMedicalControl, hasAbsence, hasTracking, absenceWasProcessed);
				},
				(error) => {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			);
		} else {
			this.openDialogDelete(medicalControlTracking, isMedicalControl, hasAbsence, hasTracking, false);
		}
	}

	openDialogDelete(
		element: MedicalControl | MedicalControlChild,
		isMedicalControl: boolean,
		hasAbsence: boolean,
		hasTracking: boolean,
		absenceWasProcessed: boolean
	): void {
		const type = isMedicalControl ? "MedicalControl" : "MedicalControlChild";
		this.medicalControlFunctionService.openDialogDelete(hasAbsence, hasTracking, absenceWasProcessed).subscribe((accept) => {
			if (accept) {
				this.spinner.show();
				element._type = type;
				this.medicalControlFunctionService
					.delete(element)
					.then(() => {
						this.successDelete(element.id);
					})
					.catch((error) => {
						this.alertService.error(error.error.Error, "medicalControlError");
					})
					.finally(() => {
						this.spinner.hide();
					});
			}
		});
	}

	successDelete(id: number): void {
		if (this.idMedicalControl === id) {
			this.goList();
		} else {
			this.getMedicalControl(this.idMedicalControl);
		}
		this.snackBarService.openSnackBar({
			message: "deletedSuccessfully",
			icon: true,
			action: null,
			secondsDuration: 5
		});
	}

	goList(): void {
		this.router.navigateByUrl(`/healthApp/medical-controls`);
	}

	getControlTypes(): Promise<Select[]> {
		return new Promise((resolve, reject) => {
			this.medicalControlTypesService
				.get()

				.subscribe(
					(data) => {
						resolve(data);
					},
					(error) => {
						if (error.status !== 404) {
							this.alertService.error(error.error.Error, "medicalControlError");
						} else {
							this.controlTypes = [];
							reject();
						}
					}
				);
		});
	}

	getTrackingTypes(): Promise<Select[]> {
		return new Promise((resolve, reject) => {
			this.medicalControlTrackingTypeService
				.get()

				.subscribe(
					(data) => {
						resolve(data);
					},
					(error) => {
						if (error.status !== 404) {
							this.alertService.error(error.error.Error, "medicalControlError");
						} else {
							this.trackingType = [];
							reject();
						}
					}
				);
		});
	}

	getActionTypes(): Promise<Select[]> {
		return new Promise((resolve, reject) => {
			this.medicalControlActionTypesService
				.getActions()

				.subscribe(
					(data) => {
						resolve(data);
					},
					(error) => {
						if (error.status !== 404) {
							this.alertService.error(error.error.Error, "medicalControlError");
						} else {
							this.actionTypes = [];
							reject();
						}
					}
				);
		});
	}

	getMedicalControl(id: number): void {
		const tracking = true;
		this.spinner.show();
		this.medicalControlService
			.get(id, tracking)
			.subscribe(
				(data: any) => {
					this.addExternalData(data).then((medicalControl) => {
						this.medicalControlDataSource.data = [medicalControl];
						this.medicalControlTrackingDataSource.data = medicalControl.tracking;
						this.medicalControlTrackingDataSource.paginator = this.paginator;
						this.medicalControlTrackingDataSource.sort = this.sort;
						this.isDeleteTracking = Math.max.apply(
							Math,
							data.tracking.map((x) => {
								return x.id;
							})
						);
					});
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalControlError");
					} else {
						this.medicalControlDataSource.data = [];
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	getRestDays(id: number): Promise<number> {
		let restDays = 0;
		return new Promise((resolve, reject) => {
			this.absenceService.getById(id).subscribe(
				(data: AbsenceRequest) => {
					const dateFrom = moment(data.dateFrom, "YYYY-MM-DD");
					const dateTo = moment(data.dateTo, "YYYY-MM-DD");
					restDays = dateTo.diff(dateFrom, "days") + 1;
					resolve(restDays);
				},
				(error) => {
					reject(error);
				}
			);
		});
	}

	addExternalData(data: any): Promise<any> {
		return new Promise((resolve) => {
			const promises: Array<Promise<any>> = [];
			const medicalControl = {
				id: data.id,
				controlDate: data.date,
				controlTypeDescription: "",
				controlActionTypeDescription: "",
				controlRestDays: 0,
				absence: {
					id: data.idAbsence
				},
				tracking: []
			};

			if (data.idAbsence) {
				promises.push(
					this.getRestDays(data.idAbsence)
						.then((days) => {
							medicalControl.controlRestDays = days;
						})
						.catch(() => {
							medicalControl.controlRestDays = 0;
						})
				);
			}

			const controlDescription = this.controlTypes.find((x) => x.id === data.idControlType).description;
			medicalControl.controlTypeDescription = controlDescription;

			const actionTypeDescription = this.actionTypes.find((x) => x.id === data.idAction).description;
			medicalControl.controlActionTypeDescription = actionTypeDescription;

			data.tracking.forEach((element) => {
				const tracking = {
					trackingDate: element.date,
					trackingDescription: "",
					trackingControlTypeDescription: "",
					trackingRestDays: 0,
					id: element.id,
					idAbsence: element.idAbsence
				};
				if (element.idAbsence) {
					promises.push(
						this.getRestDays(element.idAbsence)
							.then((days) => {
								tracking.trackingRestDays = days;
							})
							.catch(() => {
								medicalControl.controlRestDays = 0;
							})
					);
				}
				medicalControl.tracking.push(tracking);
				const trackingTypeDescription = this.controlTypes.find((x) => x.id === element.idControlType).description;
				tracking.trackingControlTypeDescription = trackingTypeDescription;

				const trackingDescription = this.trackingType.find((x) => x.id === element.idTrackingType).description;
				tracking.trackingDescription = trackingDescription;
			});
			Promise.all(promises).then(() => {
				resolve(medicalControl);
			});
		});
	}

	addTracking(): void {
		const params: any = { parent: this.idMedicalControl };
		this.router.navigate(["healthApp", "medical-control", "create"], { queryParams: params });
	}
}
