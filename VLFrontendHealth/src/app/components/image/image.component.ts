import { Component, OnInit, ViewChild, Input, Output, EventEmitter, OnDestroy } from "@angular/core";
import { takeUntil } from "rxjs/internal/operators/takeUntil";
import { Subject } from "rxjs/internal/Subject";

import { FileManagementService } from "../../services/file-management.service";
import { LoadingSpinnerComponent } from "../loading-spinner/loading-spinner.component";
import { ImageView } from "./../../models/image-view.model";

@Component({
	selector: "app-image",
	templateUrl: "./image.component.html",
	styleUrls: ["./image.component.scss"]
})
export class ImageComponent implements OnInit, OnDestroy {
	@Input() showSpinner = true;

	@Output() onError: EventEmitter<string[]> = new EventEmitter<string[]>();

	@Input() hideButton: boolean;

	@Input() set imageDefault(url: string) {
		if (url) {
			this.sourceFile = url;
		}
	}
	@Input() set entityTypeId(entityTypeId: number) {
		this.idEntityType = entityTypeId;
	}
	@Input() set fileTypeId(fileTypeId: number) {
		this.idFileType = fileTypeId;
	}
	@Input() set entityId(entityId: number) {
		this.idEntity = entityId;
		if (entityId) {
			this.getImage();
		}
	}

	@Input() id: string;
	@Input() class = "avatar-employee";
	@Input() styleInner: string;

	@Input() set dataImage(value: ImageView) {
		if (value) {
			this.idEntity = value.idEntity;
			this.idEntityType = value.entityTypeId;
			this.idFileType = value.fileTypeId;
			this.active = value.active;
			this.type = value.type;
			this.fileId = value.fileId;
			this.getImage();
		}
	}

	@ViewChild("spinnerImage", { static: true })
	public spinner: LoadingSpinnerComponent;

	public sourceFile = "/assets/img/avatar-empty.png";

	public idEntity: number;
	public idEntityType: number;
	public idFileType: number;
	public active: boolean;
	public type: string;
	public fileId: number;
	protected ngUnsubscribe$: Subject<void> = new Subject<void>();
	private fileToUpload: File = null;

	constructor(public fileUploadService: FileManagementService) {}

	ngOnInit(): void {}

	getImage(): void {
		if (this.showSpinner) {
			this.spinner.show();
		}
		this.fileUploadService
			.getfile(this.idEntityType, this.idEntity, this.idFileType, this.active, this.type, this.fileId)
			.pipe(takeUntil(this.ngUnsubscribe$))
			.subscribe((files) => {
				this.sourceFile = files.totalCount > 0 ? files.values[0].url : this.sourceFile;
			})
			.add(() => {
				if (this.showSpinner) {
					this.spinner.hide();
				}
			});
	}

	addFile(files: FileList): void {
		this.fileToUpload = files.item(0);

		const reader = new FileReader();
		reader.readAsDataURL(files[0]);
		reader.onload = (_event) => {
			this.sourceFile = <string>reader.result;
		};

		this.sendFile();
	}

	sendFile(): void {
		this.spinner.show();

		this.fileUploadService
			.postFile(this.fileToUpload, this.idEntityType, this.idEntity, this.idFileType)
			.then()
			.catch(() => {
				this.onError.emit(["uploadFileError"]);
			})
			.finally(() => {
				this.spinner.hide();
			});
	}

	preview(url: string): void {
		this.fileUploadService.viewImage(url, {}).then((xhr) => (this.sourceFile = xhr));
	}
	public ngOnDestroy(): void {
		this.ngUnsubscribe$.next();
		this.ngUnsubscribe$.complete();
	}
}
