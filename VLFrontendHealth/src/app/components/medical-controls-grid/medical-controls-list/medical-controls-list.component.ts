import { OnInit, AfterViewInit, Component, Input, ViewChild } from "@angular/core";
import { MatPaginator } from "@angular/material/paginator";
import { MatTableDataSource } from "@angular/material/table";
import { MatSort } from "@angular/material/sort";
import { Router } from "@angular/router";
import * as moment from "moment";

import { SearchEngineFilterService } from "./../../../services/search-engine-filter.service";
import { UserPreference } from "./../../../models/user-preference.model";
import { AlertService } from "../../../services/alert.service";
import { ExcelService } from "../../../services/excel.services";
import { ElipsisAction, IconTypes } from "../../elipsis-grid/elipsis-grid.component";
import { LoadingSpinnerComponent } from "../../loading-spinner/loading-spinner.component";
import { MedicalControlRow, MedicalControlList, MedicalControlsFilterParameters } from "../../../models/medical-control.model";
import { ModalService } from "./../../../services/modal.service";
import { SnackBarService } from "./../../../services/snack-bar.service";
import { MedicalControlsService } from "./../../../services/medical-controls.service";
import { MedicalControl } from "./../../../models/medical-control.model";
import { EnumMedicalControlActions } from "../../../models/medicalControlAction.enum";
import { MedicalControlFunctionsService } from "./../../../services/medical-control-functions.service";
import { TranslatePipe } from "./../../../pipes/translate.pipe";
import { Select } from "../../../models/select.model";
import { PathologyService } from "./../../../services/pathology.service";

@Component({
	selector: "app-medical-control-list",
	templateUrl: "./medical-controls-list.component.html",
	styleUrls: ["./medical-controls-list.component.scss"]
})
export class MedicalControlsListComponent implements OnInit, AfterViewInit {
	@Input() itemsPerPage: number;
	public actions: ElipsisAction[];

	@Input() fromMedicalHistory = false;
	@Input() idEmployee = 0;

	public enableEditing = false;
	@Input("editing") set EnableEditing(value: boolean) {
		this.setEditingStatus(value);
	}

	@ViewChild("spinnerList", { static: true })
	public spinner: LoadingSpinnerComponent;

	@ViewChild(MatSort, { static: false })
	public sort: MatSort;

	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	public pageSizeOptions: number[];

	public quantityMedicalControls: number;
	public medicalControl: MedicalControl;
	public showDetail = false;

	public actionsAllowTraking = [
		EnumMedicalControlActions.absence,
		EnumMedicalControlActions.inItinere,
		EnumMedicalControlActions.professionalIllness,
		EnumMedicalControlActions.workAccident,
		EnumMedicalControlActions.complaintInItinere,
		EnumMedicalControlActions.complaintProfessionalIllness,
		EnumMedicalControlActions.complaintWorkAccident
	];
	public actionsAllowComplaint = [
		EnumMedicalControlActions.inItinere,
		EnumMedicalControlActions.professionalIllness,
		EnumMedicalControlActions.workAccident,
		EnumMedicalControlActions.complaintInItinere,
		EnumMedicalControlActions.complaintProfessionalIllness,
		EnumMedicalControlActions.complaintWorkAccident
	];

	public get medicalControlsFilter(): MedicalControlsFilterParameters {
		return this._medicalControlsFilter;
	}
	public set medicalControlsFilter(value: MedicalControlsFilterParameters) {
		if (value && value.pageSize) {
			this._medicalControlsFilter = value;
			this._medicalControlsFilter.pageSize = this.itemsPerPage;
			this.setPageSizeOptions();
		}
	}

	public actionTypeIds = [
		EnumMedicalControlActions.examination,
		EnumMedicalControlActions.swabbing,
		EnumMedicalControlActions.complaintProfessionalIllness,
		EnumMedicalControlActions.complaintInItinere,
		EnumMedicalControlActions.complaintWorkAccident,
		EnumMedicalControlActions.break
	];

	public pathologies = new Array<Select>();
	public selectedIdMedicalControl: number;

	medicalControlsDisplayedColumns: string[] = [
		"fileNumber",
		"fullName",
		"controlType",
		"action",
		"dateRange",
		"duration",
		"absenceType",
		"pathology",
		"options"
	];

	medicalControlsDataSource: MatTableDataSource<MedicalControlRow>;

	private _medicalControlsFilter = new MedicalControlsFilterParameters();

	constructor(
		private searchEngineFilterService: SearchEngineFilterService,
		private medicalControlsService: MedicalControlsService,
		private pathologyService: PathologyService,
		private alertService: AlertService,
		private excelService: ExcelService,
		private router: Router,
		private modalService: ModalService,
		private snackBarService: SnackBarService,
		private medicalControlFunctionService: MedicalControlFunctionsService,
		private translatePipe: TranslatePipe,
		public userPreference: UserPreference
	) {
		this.medicalControlsDataSource = new MatTableDataSource<MedicalControlRow>();
	}

	ngOnInit(): void {
		this.medicalControlsDisplayedColumns = this.fromMedicalHistory
			? this.medicalControlsDisplayedColumns.splice(2, 8)
			: this.medicalControlsDisplayedColumns;
	}

	setEditingStatus(editing: boolean): void {
		this.enableEditing = editing;
	}

	ngAfterViewInit(): void {
		this.medicalControlListElipsis();
	}

	medicalControlListElipsis(): void {
		this.actions = new Array<ElipsisAction>();
		this.actions.push(
			{
				action: (element) => this.edit(element.idMedicalControl),
				icon: IconTypes.edit
			},
			{
				action: (element) => this.viewDetail(element.idMedicalControl),
				icon: IconTypes.follow,
				condition: (element) => {
					return this.actionsAllowTraking.some((x) => element.action.idAction === x);
				}
			},
			{
				action: (element) => this.complaint(element.idMedicalControl),
				icon: IconTypes.complaint,
				condition: (element) => {
					return this.actionsAllowComplaint.some((x) => element.action.idAction === x);
				}
			},
			{
				action: (element) => this.delete(element),
				icon: IconTypes.delete
			}
		);
	}

	getPathologies(): Promise<void> {
		return new Promise((resolve) => {
			this.spinner.show();
			this.pathologyService
				.getAll()
				.subscribe((data: Select[]) => {
					this.pathologies = data;

					resolve();
				})
				.add(() => {
					this.spinner.hide();
				});
		});
	}

	complaint(id: number): void {
		this.router.navigateByUrl(`/healthApp/medical-control/${id}/complaint`);
	}

	edit(id: number): void {
		if (this.enableEditing) {
			this.selectedIdMedicalControl = id;
			this.router.navigateByUrl(this.getUrlForRoute());
		}
	}

	dblclickMedicalControl(id: number): void {
		this.edit(id);
	}

	getUrlForRoute(): string {
		let url = `/healthApp/medical-control/${this.selectedIdMedicalControl}`;

		if (this.fromMedicalHistory) {
			url = `/healthApp/medical-control/${this.selectedIdMedicalControl}?medicalHistoryIdEmployee=${this.idEmployee}`;
		}
		return url;
	}

	exportAsXLSX(): void {
		this.spinner.show();
		let filteredMedicalControl = [];

		const filter = Object.assign({}, this._medicalControlsFilter);
		filter.pageSize = this.quantityMedicalControls;

		this.searchEngineFilterService
			.getMedicalControlByFilter(filter)
			.subscribe(
				(data: MedicalControlList) => {
					const columnsDelete = [
						"idMedicalControl",
						"absenceDateStart",
						"absenceDateEnd",
						"idAbsence",
						"idMedicalControlParent",
						"lastName",
						"name",
						"viewOptions"
					];
					filteredMedicalControl = data.paginationList.map((value) => ({
						fileNumber: value.fileNumber,
						fullName: `${value.lastName}, ${value.name}`,
						controlType: value.controlType.controlTypeDescription,
						action: value.action.actionDescription,
						range: this.getRangeColumnDescription(value),
						duration: this.getDurationColumnDescription(value),
						absenceType: this.getAbsenceTypeDescription(value),
						pathology: this.getPathologyDescription(value)
					}));
					this.excelService.exportAsExcelFile(filteredMedicalControl, "MedicalControls", columnsDelete);
				},
				(error) => {
					if (error.status === 404) {
						this.medicalControlsDataSource.data = [];
						this.quantityMedicalControls = 0;
					} else {
						this.alertService.error(error.error.Error, "medicalControlError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	handlePage(page: any): void {
		this.medicalControlsFilter.page = page.pageIndex;
		this.medicalControlsFilter.pageSize = page.pageSize;
		this.getMedicalControlByFilter(true);
	}

	setPageSizeOptions(): void {
		this.paginator.pageSize = this.itemsPerPage;
		this.paginator.initialized.subscribe(
			() => {
				const countOptions = 5;
				this.pageSizeOptions = new Array<number>();
				for (let i = 1; i <= countOptions; i++) {
					this.pageSizeOptions.push(this.itemsPerPage * i);
				}
			},
			(error) => {
				this.alertService.error(error.error.Error, "medicalControlError");
			}
		);
	}

	getMedicalControlByFilter(handlePage: boolean = false): void {
		this.spinner.show();
		this.searchEngineFilterService
			.getMedicalControlByFilter(this._medicalControlsFilter)
			.subscribe(
				(data: MedicalControlList) => {
					if (!handlePage) {
						this.quantityMedicalControls = data.quantity;
						this.paginator.firstPage();
					}
					this.pathologies.length
						? this.dataPaginationListMap(data)
						: this.getPathologies().then(() => {
								// tslint:disable-next-line: ter-indent
								this.dataPaginationListMap(data);
						  });
				},
				(error) => {
					if (error.status === 404) {
						this.medicalControlsDataSource.data = [];
						this.quantityMedicalControls = 0;
					} else {
						this.alertService.error(error.error.Error, "medicalControlError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	dataPaginationListMap(data: MedicalControlList): void {
		this.medicalControlsDataSource.data = data.paginationList.map((value) => ({
			...value,
			fullName: `${value.lastName}, ${value.name}`,
			range: this.getRangeColumnDescription(value),
			duration: this.getDurationColumnDescription(value),
			absenceTypeDescription: this.getAbsenceTypeDescription(value),
			pathology: this.getPathologyDescription(value)
		}));
	}

	sortData(sort: MatSort): void {
		const sortByDefault = sort.active;
		const sortBy = {
			action: "ActionDescription",
			controltype: "ControlTypeDescription",
			fullname: "LastName",
			daterange: "AbsenceDateStart",
			absencetype: "IdType"
		};
		const field = sortBy[sort.active.toLowerCase()] || sortByDefault;
		const sorter = `${field} ${sort.direction}`;

		this.medicalControlsFilter.orderBy = sorter;
		this.getMedicalControlByFilter();
	}

	delete(element: MedicalControlRow): void {
		const absenceAction = this.actionsAllowTraking.some((x) => element.action.idAction === x);
		if (absenceAction) {
			this.getMedicalControlData(element.idMedicalControl)
				.then((data: MedicalControl) => {
					this.deleteMedicalControlAndTracking(data);
				})
				.catch((error) => {
					this.alertService.error(error.error.Error, "medicalControlError");
				});
		} else {
			this.deleteMedicalControl(element.idMedicalControl);
		}
	}

	deleteMedicalControlAndTracking(element: MedicalControl): void {
		if (element.absence?.id > 0) {
			this.medicalControlFunctionService.hasProcessedAbsence(element).subscribe((absenceWasProcessed: boolean) => {
				this.deleteMedicalControlAndTrackingMessage(element, absenceWasProcessed);
			});
		} else {
			this.deleteMedicalControlAndTrackingMessage(element);
		}
	}

	deleteMedicalControlAndTrackingMessage(element: MedicalControl, absenceWasProcessed: boolean = false): void {
		this.medicalControlFunctionService
			.openDialogDelete(element.absence.id !== null, element.tracking.length > 0, absenceWasProcessed)
			.subscribe(
				(accept) => {
					if (accept) {
						this.spinner.show();
						this.medicalControlFunctionService
							.delete(element)
							.then(() => {
								this.showSnackBar();
							})
							.catch((error) => {
								this.alertService.error(error.error.Error, "medicalControlError");
							})
							.finally(() => {
								this.spinner.hide();
							});
					}
				},
				(error) => {
					this.alertService.error(error.error.Error, "medicalControlError");
				}
			);
	}

	deleteMedicalControl(id: number): void {
		this.modalService
			.openDialog({
				title: "atention",
				message: "areYouSureForDelete",
				noButtonMessage: "cancel",
				okButtonMessage: "yesIAmSure"
			})
			.subscribe((accept) => {
				if (accept) {
					this.spinner.show();
					this.medicalControlFunctionService
						.deleteMedicalControl(id)
						.then(() => {
							this.showSnackBar();
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

	viewDetail(idMedicalControl: number): void {
		this.router.navigateByUrl(`/healthApp/medical-control/detail/${idMedicalControl}`);
	}

	showSnackBar(): void {
		this.snackBarService.openSnackBar({
			message: "deletedSuccessfully",
			icon: true,
			action: null,
			secondsDuration: 5
		});
		this.getMedicalControlByFilter();
	}

	getMedicalControlData(id: number): Promise<MedicalControl> {
		return new Promise((resolve, reject) => {
			this.spinner.show();
			const tracking = true;
			this.medicalControlsService
				.get(id, tracking)
				.subscribe(
					(data: any) => {
						const medicalControl: MedicalControl = {
							...data,
							absence: {
								id: data.idAbsence
							}
						};
						resolve(medicalControl);
					},
					(error) => {
						reject(error);
					}
				)
				.add(() => {
					this.spinner.hide();
				});
		});
	}

	getRangeColumnDescription(medicalControlRow: MedicalControlRow): string {
		if (medicalControlRow.idAbsence) {
			const startDate = moment(medicalControlRow.absenceDateStart);
			const absenceStartDate = startDate.format(this.userPreference.dateFormat.format.toUpperCase());
			const endDate = moment(medicalControlRow.absenceDateEnd);
			const absenceEndDate = endDate.format(this.userPreference.dateFormat.format.toUpperCase());
			return `${absenceStartDate ? absenceStartDate : ""} - ${absenceEndDate ? absenceEndDate : ""}`;
		}

		if (medicalControlRow.breakTime) {
			const controlDate = moment(medicalControlRow.controlDate);
			const resultDate = controlDate.format(this.userPreference.dateFormat.format.toUpperCase());
			return resultDate;
		}

		if (!medicalControlRow.idAbsence && !medicalControlRow.breakTime) {
			const consultDate = moment(medicalControlRow.controlDate);
			const resultDate = consultDate.format(this.userPreference.dateFormat.format.toUpperCase());
			return `${resultDate} `;
		}
	}

	getDurationColumnDescription(medicalControlRow: MedicalControlRow): string {
		if (medicalControlRow.idAbsence) {
			const date1 = new Date(medicalControlRow.absenceDateStart);
			const date2 = new Date(medicalControlRow.absenceDateEnd);
			const diff = Math.abs(date1.getTime() - date2.getTime());
			const diffDays = Math.ceil(diff / (1000 * 3600 * 24)) + 1;
			return `${diffDays} ${this.translatePipe.transform("days")}`;
		}
		if (medicalControlRow.breakTime) {
			return `${medicalControlRow.breakTime} ${this.translatePipe.transform("minutes")}`;
		}
		const oneDay = 1;
		return `${oneDay} ${this.translatePipe.transform("day")}`;
	}

	getAbsenceTypeDescription(data: MedicalControlRow): string {
		if (this.actionTypeIds.some((item) => item === data.action.idAction)) {
			return `${this.translatePipe.transform("noApply")}`;
		}
		return data.absenceType?.name;
	}

	getPathologyDescription(data: MedicalControlRow): string {
		if (data.idPathology) {
			const pathologyDescription = this.pathologies.find((x) => x.id === data.idPathology).description;
			return pathologyDescription;
		}
		return `${this.translatePipe.transform("noApply")}`;
	}
}
