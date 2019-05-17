import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { EventoService } from 'src/app/_services/evento.service';
import { BsLocaleService } from 'ngx-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { Evento } from 'src/app/_models/Evento';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-evento-edit',
  templateUrl: './evento-edit.component.html',
  styleUrls: ['./evento-edit.component.css']
})
export class EventoEditComponent implements OnInit {
  titulo = 'Editar Evento';
  evento: Evento = new Evento();
  registerForm: FormGroup;
  imagemURL = 'assets/img/img-upload.png';
  fileNameToUpdate: string;
  dataAtual = '';
  file: File;

  get lotes(): FormArray {
    return <FormArray>this.registerForm.get('lotes');
  }

  get redesSociais(): FormArray {
    return <FormArray>this.registerForm.get('redesSociais');
  }

  constructor(private eventoService: EventoService,
              private fb: FormBuilder,
              private localeService: BsLocaleService,
              private toastr: ToastrService,
              private router: ActivatedRoute) {
      this.localeService.use('pt-br');
    }

    ngOnInit() {
      this.validation();
      this.carregarEvento();
    }

    carregarEvento() {
      const idEvento = +this.router.snapshot.paramMap.get('id');
      this.eventoService.getEventoById(idEvento)
      .subscribe(
        (evento: Evento) => {
          this.evento = Object.assign({}, evento);
          this.fileNameToUpdate = evento.imagemURL.toString();

          this.imagemURL = `http://localhost:5000/resources/images/${evento.imagemURL}?_ts=${this.dataAtual}`;
          this.evento.imagemURL = '';
          this.registerForm.patchValue(this.evento);

          this.evento.lotes.forEach(lote => {
            this.lotes.push(this.criaLote(lote));
          });
          this.evento.redesSociais.forEach(redeSocial => {
            this.redesSociais.push(this.criaRedeSocial(redeSocial));
          });
        }
      );
    }

    validation() {
      this.registerForm = this.fb.group({
        id: [],
        tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
        local: ['', [Validators.required]],
        dataEvento: ['', [Validators.required]],
        qtdPessoa: ['', [Validators.required, Validators.max(120000)]],
        imagemURL: [''],
        telefone: ['', [Validators.required]],
        email: ['', [Validators.required, Validators.email]],
        lotes: this.fb.array([]),
        redesSociais: this.fb.array([])
      });
    }

    criaLote(lote: any): FormGroup {
      return this.fb.group({
        id: [lote.id],
        nome: [lote.nome, [Validators.required]],
        quantidade: [lote.quantidade, [Validators.required]],
        preco: [lote.preco, [Validators.required]],
        dataInicio: [lote.dataInicio],
        dataFim: [lote.dataFim]
      });
    }

    criaRedeSocial(redeSocial: any): FormGroup {
      return this.fb.group({
        id: [redeSocial.id],
        nome: [redeSocial.nome, [Validators.required]],
        url: [redeSocial.url, [Validators.required]]
      });
    }

    onFileChange(file: FileList) {
      const reader = new FileReader();

      reader.onload = (event: any) => this.imagemURL = event.target.result;

      this.file = (<HTMLInputElement>event.target).files[0];
      reader.readAsDataURL(file[0]);
    }

    adicionarLote() {
      this.lotes.push(this.criaLote({id: 0}));
    }

    adicionarRedeSocial() {
      this.redesSociais.push(this.criaRedeSocial({id: 0}));
    }

    removerLote(id: number) {
      this.lotes.removeAt(id);
    }

    removerRedeSocial(id: number) {
      this.redesSociais.removeAt(id);
    }

    salvarEvento() {
      this.evento = Object.assign({id: this.evento.id}, this.registerForm.value);
      this.evento.imagemURL = this.fileNameToUpdate;

      this. uploadImagem();

      this.eventoService.putEvento(this.evento)
      .subscribe((novoEvento: Evento) => {
        console.log(novoEvento);

        this.toastr.success('Editado com sucesso.');
      }, error => {
        this.toastr.error(`Erro ao editar: ${error}`);
        console.log(error);
      });
    }

    uploadImagem() {
      if (this.registerForm.get('imagemURL').value !== '') {
        this.eventoService.postUpload(this.file, this.fileNameToUpdate).subscribe(
          () => {
            this.dataAtual = new Date().getMilliseconds().toString();
            this.imagemURL = `http://localhost:5000/resources/images/${this.evento.imagemURL}?_ts=${this.dataAtual}`;
          }
        );
      }
    }
}
