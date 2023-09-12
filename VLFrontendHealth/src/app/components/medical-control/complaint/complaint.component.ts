import { AfterViewInit, Component, OnInit, ViewChild, OnDestroy } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { DragDropFileService } from "src/app/services/drag-drop-file.service";
import { FileManagementService } from "src/app/services/file-management.service";
import { MedicalControlsService } from "src/app/services/medical-controls.service";

import { AlertService } from "../../../services/alert.service";
import { ModalService } from "../../../services/modal.service";
import { SuccessService } from "../../../services/success.service";
import { ElipsisAction, IconTypes } from "../../elipsis-grid/elipsis-grid.component";
import { LoadingSpinnerComponent } from "../../loading-spinner/loading-spinner.component";
import { MedicalControl } from "./../../../models/medical-control.model";
import { DragDropFileComponent } from "./../../../shared/components/drag-drop-file/drag-drop-file.component";
import { MenuService } from "../../../services/menu.service";
import { TranslatePipe } from "../../../pipes/translate.pipe";

@Component({
	selector: "app-complaint",
	templateUrl: "./complaint.component.html",
	styleUrls: ["./complaint.component.scss"]
})
export class ComplaintComponent implements OnInit, AfterViewInit, OnDestroy {
	@ViewChild("complaintSpinner", { static: false })
	public complaintSpinner: LoadingSpinnerComponent;

	public formComplaint: FormGroup;
	public idMedicalControl: number;
	public idEmployee: number;
	public fileTypeId = 10002;
	public idFileComplaint: number;
	public hasFileComplaint: boolean;
	@ViewChild("dragAndDropFile", { static: false })
	public dragAndDropFile: DragDropFileComponent;
	public ellipsisActions = new Array<ElipsisAction>();
	public acceptFormats: string[] = [".png", ".jpg", ".pdf", ".jpeg"];
	public active = false;
	public fileType = "file";
	public entityTypeIdEmployee = 1;
	public updateData: any;

	constructor(
		private fb: FormBuilder,
		private modalService: ModalService,
		private route: ActivatedRoute,
		private router: Router,
		private successService: SuccessService,
		private alertService: AlertService,
		private medicalControlService: MedicalControlsService,
		private fileManagementService: FileManagementService,
		private dragDropFileService: DragDropFileService,
		private menuService: MenuService,
		private translatePipe: TranslatePipe
	) {}

	ngOnInit(): void {
		this.menuService.hideMenu();
		this.formComplaint = this.fb.group({
			id: [0],
			idFile: [null],
			filename: [null, [Validators.required]]
		});

		this.setEllipsis();
	}

	ngAfterViewInit(): void {
		this.route.paramMap.subscribe((params) => {
			this.idMedicalControl = Number(params.get("id"));
		});
		this.getComplaint();
	}

	ngOnDestroy(): void {
		this.menuService.showMenu();
	}

	getComplaint(): void {
		this.medicalControlService
			.get(this.idMedicalControl)
			.subscribe((data: MedicalControl) => {
				this.idEmployee = data.idEmployee;
				this.idFileComplaint = data.idFileComplaint;
				this.hasFileComplaint = data.idFileComplaint !== null;
				if (this.hasFileComplaint) {
					this.getDataFile();
				}
			})
			.add((error) => {
				if (error.status !== 404) {
					this.alertService.error(error.error.Error, "complaintError");
				}
			});
	}

	getDataFile(): void {
		this.fileManagementService
			.getfile(this.entityTypeIdEmployee, this.idEmployee, this.fileTypeId, this.active, this.fileType, this.idFileComplaint)
			.subscribe((res) => {
				const description = res.values[0].description;
				const iconClass = `file-${this.dragDropFileService.getExtensionFile(res.values[0].url).replace(".", "")}`;
				const fileData = {
					file: {
						file: res.values[0],
						urlName: this.dragDropFileService.getFileName(res.values[0].url),
						iconClass: iconClass,
						url: res.values[0].url,
						idFile: res.values[0].fileId,
						description: description
					}
				};
				this.updateData = fileData;
			});
	}

	setIdCertificate(idFile: number): void {
		this.formComplaint.get("idFile").setValue(idFile);
		this.idFileComplaint = idFile;
		this.formComplaint.markAsDirty();
	}

	cancel(): void {
		this.formComplaint.dirty ? this.returnWithConfirmation() : this.router.navigateByUrl(this.returnToGrid());
	}

	returnWithConfirmation(): void {
		this.modalService
			.openDialog({
				title: "attention",
				message: "AreYouSureYouWantToGoOut",
				noButtonMessage: "cancel",
				okButtonMessage: "yesIAmSure"
			})
			.subscribe((accept) => {
				if (accept) {
					this.router.navigateByUrl(this.returnToGrid());
				}
			});
	}

	save(): void {
		if (!this.formComplaint.valid) {
			this.formComplaint.markAllAsTouched();
			return;
		}
		this.dragAndDropFile.save().then(() => {
			this.complaintSpinner.show();
			this.setIdCertificate(this.dragAndDropFile.fileId);
			this.applyPatch();
		});
	}

	applyPatch(): void {
		const sendData = [
			{
				value: `${this.idFileComplaint}`,
				path: `/IdFileComplaint`,
				op: "replace",
				from: null
			}
		];
		this.medicalControlService
			.patch(this.idMedicalControl, sendData)
			.subscribe(
				() => {
					this.successMessage();
				},
				(error) => {
					this.alertService.error(error.error.Error, "complaintError");
				}
			)
			.add(() => {
				this.complaintSpinner.hide();
			});
	}

	successMessage(): void {
		this.successService.data.actionContinue = null;
		this.successService.data.routeContinue = null;
		this.successService.data.actionFinish = "finish";
		this.successService.data.routeFinish = this.returnToGrid();
		this.successService.data.description = this.translatePipe.transform("complaintWasSuccessfullyCreated");
		this.router.navigateByUrl("healthApp/success");
	}

	returnToGrid(): string {
		return "healthApp/medical-controls";
	}

	setEllipsis(): void {
		this.ellipsisActions.push(
			{
				action: () => {
					this.dragAndDropFile.openViewer();
				},
				icon: IconTypes.view,
				description: "view"
			},
			{
				action: () => {
					this.dragAndDropFile.download();
				},
				icon: IconTypes.download,
				description: "download"
			},
			{
				action: () => {
					this.dragAndDropFile.delete();
				},
				icon: IconTypes.delete,
				description: "delete"
			}
		);
	}

	setFilename(event: Event): void {
		return this.formComplaint.get("filename").setValue(event);
	}

	errorImage(message: string[]): void {
		this.alertService.error(message, "examError");
	}

	// --------------------------------------------------------------------------
}
