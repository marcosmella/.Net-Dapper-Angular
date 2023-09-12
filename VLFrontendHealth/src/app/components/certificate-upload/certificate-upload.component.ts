import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from "@angular/core";
import { Subject } from "rxjs/internal/Subject";

import { LoadingSpinnerComponent } from "../../components/loading-spinner/loading-spinner.component";
import { UploadFileType } from "../../models/upload-file-type.model";
import { AlertService } from "../../services/alert.service";
import { FileManagementService } from "../../services/file-management.service";

@Component({
	selector: "app-certificate-upload",
	templateUrl: "./certificate-upload.component.html",
	styleUrls: ["./certificate-upload.component.scss"]
})
export class CertificateUploadComponent implements OnInit {
	@Input() showSpinner = true;

	@Output() onError: EventEmitter<string[]> = new EventEmitter<string[]>();

	@Output() idFile = new EventEmitter();

	@Input() hideButton: boolean;

	@Input() set imageDefault(url: string) {
		this.sourceFile = url;
	}

	@Input() set entityTypeId(entityTypeId: Number) {
		this.idEntityType = entityTypeId;
	}

	@Input() set fileTypeId(fileTypeId: Number) {
		this.idFileType = fileTypeId;
	}

	@Input() saved: boolean;

	@Input() set entityId(entityId: Number) {
		this.idEntity = entityId;
	}

	@Input() importFileName: string;

	@Input() id: string;

	@Input() styleInner: string;

	@ViewChild("spinnerImage", { static: true })
	public spinnerCertificate: LoadingSpinnerComponent;
	public sourceFile = "";
	public idEntity: Number;
	public idEntityType: Number = 1;
	public idFileType: Number = 10003;
	public file: string;
	public isAllowed = false;
	public active = true;
	public allowedExtensions = new Array("pdf", "jpg", "jpeg", "png");
	public acceptFileType =
		// tslint:disable-next-line:max-line-length
		"image/jpg, image/jpeg, image/png, application/pdf";
	public fileType = "IMAGE";
	public uploadFileType = UploadFileType;
	protected ngUnsubscribe$: Subject<void> = new Subject<void>();

	private fileToUpload: File = null;

	constructor(private alertService: AlertService, public fileManagementService: FileManagementService) {}

	ngOnInit(): void {}

	addFile(files: FileList): void {
		this.fileToUpload = files.item(0);
		const reader = new FileReader();
		reader.readAsDataURL(files[0]);

		reader.onload = (_event) => {
			this.sourceFile = <string>reader.result;
		};

		this.isAllowed = this.allowedFileExtension(this.fileToUpload.name);
		if (!this.isAllowed) {
			this.alertService.warning(["importFileExtension"], "warningFileExtension");
		} else {
			this.sendFile();
		}
	}

	sendFile(): void {
		this.spinnerCertificate.show();
		this.fileManagementService
			.postFile(this.fileToUpload, this.idEntityType, this.idEntity, this.idFileType, this.fileType, this.active)
			.then((data) => {
				const resp = JSON.parse(data.response);
				if (resp.success) {
					this.saved = true;
					this.idFile.emit(resp.entity.fileId);
				} else if (!resp.values) {
					this.saved = false;
				}
			})
			.catch(() => {
				this.onError.emit(["uploadFileError"]);
				this.saved = false;
			})
			.finally(() => {
				this.spinnerCertificate.hide();
			});
	}

	getFile(): void {
		this.sourceFile = "";
	}

	allowedFileExtension(filename: string): boolean {
		const extension = filename.split(".").pop();
		let allowed = false;
		for (const item of this.allowedExtensions) {
			if (item === extension) {
				allowed = true;
				this.fileType = this.uploadFileType.file;
				this.active = false;
				break;
			}
		}
		return allowed;
	}

	formatAllowFilesExtension(extensions: Array<string>): Array<string> {
		return extensions.map((x) => ` .${x.toUpperCase()}`);
	}

	// tslint:disable-next-line: use-life-cycle-interface
	public ngOnDestroy(): void {
		this.ngUnsubscribe$.next();
		this.ngUnsubscribe$.complete();
	}
}
