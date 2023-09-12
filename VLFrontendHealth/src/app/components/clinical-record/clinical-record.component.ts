import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { MatTableDataSource } from "@angular/material/table";
import { MatPaginator, PageEvent } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { Router } from "@angular/router";

import { LoadingSpinnerComponent } from "./../../components/loading-spinner/loading-spinner.component";
import { BreadCrumbService } from "./../../services/breadcrumb.service";
import { AlertService } from "./../../services/alert.service";
import { ElipsisAction, IconTypes } from "./../../components/elipsis-grid/elipsis-grid.component";
import { TranslatePipe } from "./../../pipes/translate.pipe";
import { ExcelService } from "./../../services/excel.services";
import { StructureService } from "./../../services/structure.service";
import { StructureTypeService } from "./../../services/structure-type.service";
import { Structure } from "./../../models/structure.model";
import { EmployeePagination, ClinicalRecordGrid } from "./../../models/clinical-record-filter.model";
import { ClinicalRecordFilterService } from "./../../services/clinical-record-filter.service";

@Component({
	selector: "app-clinical-record",
	templateUrl: "./clinical-record.component.html",
	styleUrls: ["./clinical-record.component.scss"],
	providers: [TranslatePipe]
})
export class ClinicalRecordComponent implements OnInit, AfterViewInit {
	@ViewChild("spinner", { static: false })
	public spinner: LoadingSpinnerComponent;

	@ViewChild(MatPaginator, { static: true })
	paginator: MatPaginator;
	pageEvent: PageEvent;

	@ViewChild("lastName", { static: false }) lastName: ElementRef;
	@ViewChild("name", { static: false }) name: ElementRef;
	@ViewChild("documentNumber", { static: false }) documentNumber: ElementRef;
	@ViewChild("fileNumber", { static: false }) fileNumber: ElementRef;

	public formEmployeeFilter: FormGroup;
	public clinicalRecordDisplayedColumns: string[] = [
		"fileNumber",
		"fullName",
		"document",
		"StructureTypeDescription_17",
		"StructureTypeDescription_23",
		"StructureTypeDescription_5",
		"options"
	];

	public clinicalRecordDataSource: MatTableDataSource<ClinicalRecordGrid>;
	public quantityEmployees: number;
	public filter = true;
	public actions: ElipsisAction[];
	public filtered = [];
	public structures: Array<Structure>;
	public structureTypes: Array<any> = [];
	public structureTypeDescription = "";
	public structureDescription = "";
	public emptyFormEmployee: string;

	constructor(
		private clinicalRecordFilterService: ClinicalRecordFilterService,
		private alertService: AlertService,
		private router: Router,
		private excelService: ExcelService,
		private translatePipe: TranslatePipe,
		private fb: FormBuilder,
		private structureService: StructureService,
		private structureTypeService: StructureTypeService,
		public breadCrumb: BreadCrumbService
	) {
		this.clinicalRecordDataSource = new MatTableDataSource<ClinicalRecordGrid>();
		this.setFilter();
	}

	ngOnInit(): void {
		this.formStateController();
		this.setFilter();
	}

	clinicalRecordElipsis(): void {
		this.actions = new Array<ElipsisAction>();
		this.actions.push({
			action: (element) => this.viewRecord(element.idPerson),
			icon: IconTypes.view,
			description: "viewRecord"
		});
	}

	ngAfterViewInit(): void {
		this.clinicalRecordElipsis();
		this.employeeFilter();
		this.breadCrumb.showBreadCrumb([
			{ label: "hrCore", path: "/hrcore" },
			{ label: "health", path: "/healthApp" }
		]);
	}

	formStateController(): void {
		this.formEmployeeFilter.valueChanges.subscribe(() => {
			this.formEmployeeFilter.markAsUntouched();
			if (this.emptyFormEmployee === JSON.stringify(this.formEmployeeFilter.value)) {
				this.formEmployeeFilter.markAsPristine();
			}
		});
	}

	filterDisabled(): boolean {
		return !this.formEmployeeFilter.dirty || !this.formEmployeeFilter.valid;
	}

	employeeFilter(handlePage: boolean = false): void {
		this.spinner.show();
		if (!handlePage) {
			const fileNumber = this.validateFileNumberWhenNull();
			this.formEmployeeFilter.get("fileNumber").setValue(fileNumber);
			this.createChip();
		}

		this.clinicalRecordFilterService
			.getClinicalRecordByFilter(this.formEmployeeFilter.value)
			.subscribe(
				(data: EmployeePagination) => {
					if (!handlePage) {
						this.quantityEmployees = data.quantity;
						this.paginator.firstPage();
					}

					this.clinicalRecordDataSource.data = data.paginationList.map((value) => ({
						...value,
						structureTypeDescription17: value.additionalAttributes.structureTypeDescription_17,
						structureTypeDescription23: value.additionalAttributes.structureTypeDescription_23,
						structureTypeDescription5: value.additionalAttributes.structureTypeDescription_5,
						fullName: this.employeeFullName(value)
					}));
				},
				(error) => {
					if (error.status === 404) {
						this.clinicalRecordDataSource.data = [];
						this.quantityEmployees = 0;
					} else {
						this.alertService.error(error.error.Error, "gridClinicalRecords");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	handlePage(page: any): void {
		this.formEmployeeFilter.get("page").setValue(page.pageIndex);
		this.formEmployeeFilter.get("pageSize").setValue(page.pageSize);
		this.employeeFilter(true);
	}

	getStructureType(): void {
		if (this.structureTypes.length === 0) {
			this.structureTypeService.get().subscribe(
				(data) => {
					this.structureTypes = data;
				},
				(error) => {
					this.alertService.error(error.error.Error, "gridClinicalRecords");
				}
			);
		}
	}

	changeStructureType(event: any): void {
		if (!this.formEmployeeFilter.controls["idStructureType"].value) {
			this.formEmployeeFilter.controls["idStructure"].setValue("");
			this.formEmployeeFilter.controls["structure"].value[0].idStructureType = "";
			this.formEmployeeFilter.controls["structure"].value[0].idStructure = "";
			this.structures = [];
			this.structureTypeDescription = "";
			this.structureDescription = "";
			return;
		}

		this.formEmployeeFilter.controls["structure"].value[0].idStructureType = this.formEmployeeFilter.controls["idStructureType"].value;
		this.structureTypeDescription = event.description;

		this.structureService.get(this.formEmployeeFilter.controls["structure"].value[0].idStructureType).subscribe(
			(data) => {
				this.structures = data;
			},
			(error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "gridEmployee");
				} else {
					this.structures = [];
				}
			}
		);
	}

	changeStructure(event: any): void {
		if (!this.formEmployeeFilter.controls["idStructure"].value || !event) {
			this.structureDescription = "";
			this.formEmployeeFilter.controls["idStructure"].setValue("");
			this.formEmployeeFilter.controls["structure"].value[0].idStructure = "";
			return;
		}

		this.formEmployeeFilter.controls["structure"].value[0].idStructure = this.formEmployeeFilter.controls["idStructure"].value.id;
		this.structureDescription = event.description;
	}

	sortData(sort: MatSort): void {
		const sortByDefault = sort.active;
		const sortBy = {
			fullname: "LastName",
			structureTypeDescription17: "StructureTypeDescription_17",
			structureTypeDescription23: "StructureTypeDescription_23",
			structureTypeDescription5: "StructureTypeDescription_5"
		};
		const field = sortBy[sort.active.toLowerCase()] || sortByDefault;

		const sorter = `${field} ${sort.direction}`;

		this.formEmployeeFilter.get("orderBy").setValue(sorter);
		this.employeeFilter();
	}

	validateFileNumberWhenNull(): any {
		const fileNumber = this.formEmployeeFilter.get("fileNumber").value === null ? "" : this.formEmployeeFilter.get("fileNumber").value;
		return fileNumber;
	}

	validateFilterProperties(name: string, value: string): void {
		if (value !== "") {
			this.filtered.push({ name: name, text: value.toUpperCase() });
		}
	}

	createChip(): void {
		this.filtered = [];

		if (this.formEmployeeFilter.get("lastName").value) {
			this.validateFilterProperties("lastName", this.translatePipe.transform("lastName"));
		}

		if (this.formEmployeeFilter.get("name").value) {
			this.validateFilterProperties("name", this.translatePipe.transform("name"));
		}

		if (this.formEmployeeFilter.get("documentNumber").value) {
			this.validateFilterProperties("documentNumber", this.translatePipe.transform("document"));
		}

		if (this.formEmployeeFilter.get("fileNumber").value) {
			this.validateFilterProperties("fileNumber", this.translatePipe.transform("employeeFile"));
		}

		if (this.formEmployeeFilter.get("idStructureType").value) {
			this.validateFilterProperties(
				"idStructureType",
				`${this.translatePipe.transform("structureType")}: ${this.structureTypeDescription}`
			);
		}

		if (this.formEmployeeFilter.get("idStructure").value) {
			this.validateFilterProperties("idStructure", `${this.translatePipe.transform("Attribute")}: ${this.structureDescription}`);
		}
	}

	removeChip(element: string): void {
		if (element !== "idStructure") {
			this.formEmployeeFilter.get(element).reset("");
		} else {
			this.changeStructure("");
		}

		if (element === "idStructureType") {
			this.changeStructureType("");
		}

		this.employeeFilter();
	}

	clearFilter(): void {
		this.setFilter();
		this.employeeFilter();
	}

	setFilter(): void {
		const healthInsuranceStructureType = 17;
		const healthInsurancePlanStructureType = 23;
		const costCenterStructureType = 5;
		const structureTypes = [];

		structureTypes.push(costCenterStructureType);
		structureTypes.push(healthInsuranceStructureType);
		structureTypes.push(healthInsurancePlanStructureType);

		this.formEmployeeFilter = this.fb.group({
			lastName: [""],
			name: [""],
			documentNumber: [""],
			fileNumber: ["", [Validators.pattern("^[0-9]*$")]],
			idStructureType: [""],
			idStructure: [""],
			structure: [[{ idStructureType: "", idStructure: "" }]],
			IdStructureTypes: [structureTypes],
			employeesOnly: true,
			page: [0],
			pageSize: [25],
			orderBy: ["fileNumber DESC"]
		});

		this.emptyFormEmployee = JSON.stringify(this.formEmployeeFilter.value);
	}

	viewRecord(idPerson: number): void {
		this.router.navigateByUrl(`/healthApp/medical-history/modify/${idPerson}`);
	}

	employeeFullName(data: any): string {
		const name = data.name ? data.name : "";
		const secondName = data.secondName ? data.secondName : "";
		const lastName = data.lastName ? data.lastName : "";
		const secondLastName = data.secondLastName ? data.secondLastName : "";
		return `${name} ${secondName} ${lastName} ${secondLastName}`;
	}

	exportAsXLSX(): void {
		this.spinner.show();
		const filter = Object.assign({}, this.formEmployeeFilter.value);
		filter.pageSize = this.quantityEmployees;
		this.clinicalRecordFilterService
			.getClinicalRecordByFilter(filter)
			.subscribe(
				(data: EmployeePagination) => {
					this.clinicalRecordDataSource.data = data.paginationList.map((value) => ({
						...value,
						structureTypeDescription17: value.additionalAttributes.structureTypeDescription_17,
						structureTypeDescription23: value.additionalAttributes.structureTypeDescription_23,
						structureTypeDescription5: value.additionalAttributes.structureTypeDescription_5,
						fullName: this.employeeFullName(value)
					}));

					const columnsDelete = ["idBossEmployee", "idPerson", "active", "filePath"];
					this.excelService.exportAsExcelFile(this.clinicalRecordDataSource.data, "FichasMedicas", columnsDelete);
				},
				(error) => {
					if (error.status === 404) {
						this.clinicalRecordDataSource.data = [];
						this.quantityEmployees = 0;
					} else {
						this.alertService.error(error.error.Error, "gridClinicalRecords");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	import(): void {
		this.setReturnUrl();
		this.router.navigateByUrl(`/massiveOperationApp/import/health`);
	}

	private setReturnUrl(): void {
		localStorage.setItem("returnUrl", this.router.url);
	}
}
