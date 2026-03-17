using System;
using System.Collections.Generic;
using System.Globalization;

// =============================================================================
// Strings.cs - UI string resources for all supported languages.
// Supported: English (default), Spanish, Portuguese, Italian, French, German.
// To add a new language: duplicate one of the blocks below, adjust the
// two-letter ISO culture code and add a case in Load().
// =============================================================================

public static class Strings
{
    // Active dictionary, loaded at startup based on the OS language
    private static Dictionary<string, string> _active;

    // Tracks the currently active language code
    public static string CurrentLanguage { get; private set; }

    // ── Load by OS culture (called at startup) ────────────────────────────────
    public static void Load()
    {
        string lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
        LoadLanguage(lang);
    }

    // ── Load a specific language by code (called by flag buttons) ─────────────
    public static void LoadLanguage(string lang)
    {
        switch (lang)
        {
            case "es": _active = Spanish();    CurrentLanguage = "es"; break;
            case "pt": _active = Portuguese(); CurrentLanguage = "pt"; break;
            case "it": _active = Italian();    CurrentLanguage = "it"; break;
            case "fr": _active = French();     CurrentLanguage = "fr"; break;
            case "de": _active = German();     CurrentLanguage = "de"; break;
            default:   _active = English();    CurrentLanguage = "en"; break;
        }
    }

    // Returns the string for the given key; falls back to the key itself if missing
    public static string Get(string key)
    {
        string value;
        return _active.TryGetValue(key, out value) ? value : key;
    }

    // =========================================================================
    // ENGLISH (default)
    // =========================================================================
    private static Dictionary<string, string> English()
    {
        return new Dictionary<string, string>
        {
            // Sidebar buttons
            { "btn_quick",        "QUICK CLEAN"         },
            { "btn_deep",         "DEEP CLEAN"          },
            { "btn_repair",       "REPAIR SYSTEM"       },
            { "btn_update",       "UPDATE APPS"         },

            // Hover descriptions
            { "hover_quick",      "► QUICK CLEAN\r\n\r\nRemoves temp files and flushes DNS cache.\r\nPerfect for daily maintenance."                                           },
            { "hover_deep",       "≡ DEEP CLEAN\r\n\r\nRemoves logs, old updates and junk.\r\nRecommended once a month."                                                      },
            { "hover_repair",     "✚ FULL REPAIR\r\n\r\nScans and repairs Disk, Files and System Image.\r\nIf errors are found, it will help you fix them."                   },
            { "hover_update",     "⚡ UPDATE APPS (WINGET)\r\n\r\nLooks for updates for your programs.\r\nShows a list so you can confirm before installing anything."         },

            // General log messages
            { "log_ready",        "System ready. Waiting for orders..."   },
            { "log_space_before", "Free space before: "                   },
            { "log_separator",    "------------------------------------------------" },
            { "log_done",         ">>> ✅ PROCESS COMPLETE."              },
            { "log_freed",        ">>> SPACE FREED: "                     },
            { "log_error",        "❌ Unexpected error: "                 },

            // Quick Clean log
            { "log_qc_start",     ">>> ► STARTING QUICK CLEAN..."        },
            { "log_qc_user_tmp",  "> User temp files deleted."            },
            { "log_qc_sys_tmp",   "> System temp files deleted."          },
            { "log_qc_dns",       "> DNS cache flushed."                  },
            { "log_qc_explorer",  "> Restarting Explorer..."              },

            // Deep Clean log
            { "log_dc_start",     ">>> ≡ STARTING DEEP CLEAN..."         },
            { "log_dc_tmp",       "> Temp files deleted."                 },
            { "log_dc_wu",        "> Managing Windows Update..."          },
            { "log_dc_wu_cache",  "> Update cache deleted."               },
            { "log_dc_wu_start",  "> Windows Update service restarted."   },
            { "log_dc_logs",      "> Clearing system logs..."             },
            { "log_dc_defender",  "> Defender history deleted."           },
            { "log_dc_prefetch",  "> Prefetch cleared."                   },

            // Repair log (no space counter — repair does not free user space)
            { "log_rep_start",    ">>> ✚ SYSTEM REPAIR (SMART ANALYSIS)" },
            { "log_rep_diag",     "Starting diagnosis..."                 },
            { "log_rep_chk",      "[1/6] Checking disk health (CHKDSK)..."                                    },
            { "log_rep_chk_ok",   "   ✅ DISK: Healthy. No physical errors."                                  },
            { "log_rep_chk_warn", "   ⚠️ DISK: Anomalies detected."                                          },
            { "log_rep_chk_sched","   --> Repair scheduled for next reboot."                                  },
            { "log_rep_sfc",      "[2/6] Checking file integrity (SFC)..."                                    },
            { "log_rep_sfc_ok",   "   ✅ FILES: Integrity verified."                                          },
            { "log_rep_sfc_fixed","   🛠️ FILES: Errors detected and repaired."                               },
            { "log_rep_sfc_err",  "   ❌ FILES: Complex errors. Running DISM to repair the image..."          },
            { "log_rep_scan",     "[3/6] DISM ScanHealth..."                                                  },
            { "log_rep_check",    "[4/6] DISM CheckHealth..."                                                 },
            { "log_rep_restore",  "[5/6] DISM RestoreHealth (this may take a few minutes)..."                 },
            { "log_rep_rest_ok",  "   ✅ IMAGE: Restore completed."                                           },
            { "log_rep_rest_warn","   ⚠️ IMAGE: There was a problem restoring the image."                     },
            { "log_rep_cleanup",  "[6/6] Cleaning up obsolete components (ComponentCleanup)..."               },
            { "log_rep_done",     ">>> ✅ REPAIR COMPLETE."                                                   },

            // Winget log
            { "log_wg_start",     "\r\n>>> ⚡ STARTING UPDATE MANAGER...\r\n"  },
            { "log_wg_search",    "Looking for Winget...\r\n"                  },
            { "log_wg_found",     "✅ Winget found.\r\n"                        },
            { "log_wg_open",      "Opening update list. Review before accepting.\r\n" },
            { "log_wg_notfound",  "❌ Error: Winget not found.\r\n"             },
            { "log_wg_attention", "   ATTENTION: REVIEW THE LIST ABOVE"        },
            { "log_wg_warning",   "   If you do NOT want to update a program, CLOSE THIS WINDOW." },
            { "log_wg_confirm",   "   To update EVERYTHING in the list, press any key..."         },
            { "log_wg_finished",  "PROCESS COMPLETE"                                              },

            // Confirmation dialogs
            { "confirm_qc_title", "Confirm Quick Clean"                                                                             },
            { "confirm_qc_msg",   "Are you sure you want to start the Quick Clean?\n\nTemp files and DNS cache will be cleared."    },
            { "confirm_dc_title", "Confirm Deep Clean"                                                                              },
            { "confirm_dc_msg",   "Do you want to start the Deep Clean?\n\nLogs, update cache and system history will be removed." },
            { "confirm_rep_title","Confirm System Repair"                                                                           },
            { "confirm_rep_msg",  "ATTENTION: This process is thorough and may take 20-30 minutes.\n\nStart diagnosis and repair?" },
            { "confirm_upd_title","Update Applications"                                                                             },
            { "confirm_upd_msg",  "Winget will open to search for updates.\n\nA list will be shown so you can confirm before installing.\nContinue?" },

            // MessageBox dialogs
            { "mb_done_title",    "Leedeo Cleaner"                                      },
            { "mb_done_msg",      "Operation complete.\n\nSpace recovered: "            },
            { "mb_rep_done_title","Leedeo Cleaner"                                      },
            { "mb_rep_done_msg",  "Repair process complete."                            },
            { "mb_chkdsk_title",  "Disk Error"                                          },
            { "mb_chkdsk_msg",    "Errors detected on the disk. Schedule repair on next reboot?" },
            { "mb_chkdsk_sched",  "Repair scheduled for next reboot."                   },
            { "mb_chkdsk_ok",     "Done"                                                },
            { "mb_wg_notfound",   "Winget not found on this PC."                        },
            { "mb_wg_error",      "Error launching Winget: "                            },
            { "mb_start_error",   "Startup error: "                                     },

            // Registry cleaner button and hover
            { "btn_registry",          "REGISTRY CLEANER"                                                                                  },
            { "hover_registry",        "🗂 REGISTRY CLEANER\r\n\r\nScans for orphaned uninstall entries, broken autostart\r\nentries and stale file references.\r\nShows a list before deleting anything." },

            // Registry cleaner log messages
            { "log_reg_start",         ">>> 🗂 REGISTRY CLEANER — SCANNING..."                                                            },
            { "log_reg_clean",         ">>> ✅ Registry is clean. No orphaned entries found."                                             },
            { "log_reg_found",         ">>> {0} orphaned entries found:"                                                                  },
            { "log_reg_confirm",       ">>> Review the list above before confirming."                                                     },
            { "log_reg_cancelled",     ">>> ℹ Operation cancelled. Nothing was deleted."                                                  },
            { "log_reg_deleting",      ">>> Deleting confirmed entries..."                                                                },
            { "log_reg_done",          ">>> ✅ Done. Deleted: {0}  |  Errors: {1}"                                                        },

            // Registry cleaner confirmation dialogs
            { "confirm_reg_title",     "Registry Cleaner"                                                                                 },
            { "confirm_reg_msg",       "This tool scans for entries that reference files no longer present on your system.\n\nIt will show you what it finds before deleting anything.\n\nProceed with the scan?" },

            // Registry cleaner result dialogs
            { "mb_reg_clean",          "The registry looks clean. No orphaned entries were found."                                        },
            { "mb_reg_confirm_title",  "Confirm Deletion"                                                                                 },
            { "mb_reg_confirm_msg",    "{0} orphaned entries were found.\n\nDelete all of them?"                                          },
            { "mb_reg_done_msg",       "{0} entries successfully deleted."                                                                },
            { "mb_reg_done_partial",   "{0} entries deleted.\n{1} could not be removed (see log for details)."                           },

            // Save log
            { "btn_save_log",     "💾 Save log"                                         },
            { "save_log_title",   "Save log file"                                       },
            { "save_log_default", "leedeo_cleaner_log"                                  },
            { "save_log_success", "Log saved successfully."                             },
            { "save_log_error",   "Could not save the log: "                            },
            { "save_log_empty",   "The log is empty, nothing to save."                  },

            // UI labels
            { "lbl_donate",       "If this tool saved you time, consider supporting the project:" },
            { "lbl_kofi",         "☕ Support on Ko-fi (Leedeo)"                                  },
            { "lbl_pc",           "PC: "                                                          },
            { "lbl_user",         "   |   USER: "                                                 },
            { "lbl_system",       "\r\nSYSTEM: "                                                  },
            { "lbl_win_detected", "Windows Detected"                                              },
            { "lbl_ready",        "Leedeo Cleaner Ready"                                          },
        };
    }

    // =========================================================================
    // SPANISH
    // =========================================================================
    private static Dictionary<string, string> Spanish()
    {
        return new Dictionary<string, string>
        {
            { "btn_quick",        "LIMPIEZA RÁPIDA"    },
            { "btn_deep",         "LIMPIEZA PROFUNDA"  },
            { "btn_repair",       "REPARAR SISTEMA"    },
            { "btn_update",       "ACTUALIZAR APPS"    },

            { "hover_quick",      "► LIMPIEZA RÁPIDA\r\n\r\nBorra temporales y caché DNS al instante.\r\nPerfecto para el mantenimiento diario."                               },
            { "hover_deep",       "≡ LIMPIEZA PROFUNDA\r\n\r\nElimina Logs, Updates viejos y basura.\r\nRecomendado usar una vez al mes."                                      },
            { "hover_repair",     "✚ REPARACIÓN TOTAL\r\n\r\nAnaliza y repara Disco, Archivos e Imagen.\r\nSi detecta errores, te ayudará a corregirlos."                     },
            { "hover_update",     "⚡ ACTUALIZAR APPS (WINGET)\r\n\r\nBusca actualizaciones para tus programas.\r\nTe mostrará una lista para que confirmes antes de instalar nada." },

            { "log_ready",        "Sistema listo. Esperando órdenes..."   },
            { "log_space_before", "Espacio libre inicial: "               },
            { "log_separator",    "------------------------------------------------" },
            { "log_done",         ">>> ✅ PROCESO FINALIZADO."            },
            { "log_freed",        ">>> ESPACIO LIBERADO: "                },
            { "log_error",        "❌ Error inesperado: "                 },

            { "log_qc_start",     ">>> ► INICIANDO LIMPIEZA RÁPIDA..."   },
            { "log_qc_user_tmp",  "> Temporales de usuario eliminados."  },
            { "log_qc_sys_tmp",   "> Temporales del sistema eliminados." },
            { "log_qc_dns",       "> Caché DNS vaciada."                 },
            { "log_qc_explorer",  "> Reiniciando Explorer..."            },

            { "log_dc_start",     ">>> ≡ INICIANDO LIMPIEZA PROFUNDA..."         },
            { "log_dc_tmp",       "> Temporales eliminados."                     },
            { "log_dc_wu",        "> Gestionando Windows Update..."              },
            { "log_dc_wu_cache",  "> Caché de actualizaciones eliminada."        },
            { "log_dc_wu_start",  "> Servicio de Windows Update reiniciado."     },
            { "log_dc_logs",      "> Eliminando logs del sistema..."             },
            { "log_dc_defender",  "> Historial de Defender eliminado."           },
            { "log_dc_prefetch",  "> Prefetch limpiado."                         },

            { "log_rep_start",    ">>> ✚ REPARACIÓN DE SISTEMA (ANÁLISIS INTELIGENTE)" },
            { "log_rep_diag",     "Iniciando diagnóstico..."                            },
            { "log_rep_chk",      "[1/6] Analizando salud del Disco (CHKDSK)..."        },
            { "log_rep_chk_ok",   "   ✅ ESTADO DISCO: Sano. Sin errores físicos."      },
            { "log_rep_chk_warn", "   ⚠️ ESTADO DISCO: Se detectaron anomalías."        },
            { "log_rep_chk_sched","   --> Reparación programada para el próximo reinicio." },
            { "log_rep_sfc",      "[2/6] Verificando integridad de archivos (SFC)..."   },
            { "log_rep_sfc_ok",   "   ✅ ESTADO ARCHIVOS: Integridad verificada."        },
            { "log_rep_sfc_fixed","   🛠️ ESTADO ARCHIVOS: Errores detectados y reparados." },
            { "log_rep_sfc_err",  "   ❌ ESTADO ARCHIVOS: Errores complejos. Continuando con DISM..." },
            { "log_rep_scan",     "[3/6] DISM ScanHealth..."                             },
            { "log_rep_check",    "[4/6] DISM CheckHealth..."                            },
            { "log_rep_restore",  "[5/6] DISM RestoreHealth (puede tardar varios minutos)..." },
            { "log_rep_rest_ok",  "   ✅ ESTADO IMAGEN: Restauración completada."        },
            { "log_rep_rest_warn","   ⚠️ ESTADO IMAGEN: Hubo un problema al restaurar la imagen." },
            { "log_rep_cleanup",  "[6/6] Limpiando componentes obsoletos (ComponentCleanup)..." },
            { "log_rep_done",     ">>> ✅ REPARACIÓN COMPLETADA."                        },

            { "log_wg_start",     "\r\n>>> ⚡ INICIANDO GESTOR DE ACTUALIZACIONES...\r\n" },
            { "log_wg_search",    "Buscando Winget...\r\n"                                },
            { "log_wg_found",     "✅ Winget localizado.\r\n"                              },
            { "log_wg_open",      "Abriendo lista de actualizaciones. Revisa antes de aceptar.\r\n" },
            { "log_wg_notfound",  "❌ Error: Winget no encontrado.\r\n"                    },
            { "log_wg_attention", "   ATENCION: REVISA LA LISTA DE ARRIBA"                },
            { "log_wg_warning",   "   Si NO quieres actualizar algun programa, CIERRA ESTA VENTANA." },
            { "log_wg_confirm",   "   Si quieres actualizar TODO, presiona una tecla..."             },
            { "log_wg_finished",  "PROCESO FINALIZADO"                                              },

            { "confirm_qc_title", "Confirmar Limpieza Rápida"                                                                              },
            { "confirm_qc_msg",   "¿Estás seguro de iniciar la Limpieza Rápida?\n\nSe borrarán archivos temporales y se vaciará la caché DNS." },
            { "confirm_dc_title", "Confirmar Limpieza Profunda"                                                                             },
            { "confirm_dc_msg",   "¿Deseas iniciar la Limpieza Profunda?\n\nSe eliminarán historiales, logs de sistema y caché de actualizaciones." },
            { "confirm_rep_title","Confirmar Reparación de Sistema"                                                                         },
            { "confirm_rep_msg",  "ATENCIÓN: Este proceso es exhaustivo y puede tardar entre 20 y 30 minutos.\n\n¿Quieres comenzar el diagnóstico y reparación?" },
            { "confirm_upd_title","Actualizar Aplicaciones"                                                                                 },
            { "confirm_upd_msg",  "Se abrirá el gestor Winget para buscar actualizaciones.\n\nSe mostrará una lista y podrás confirmar si quieres instalar todo o cancelar.\n¿Continuar?" },

            { "mb_done_title",    "Leedeo Cleaner"                                          },
            { "mb_done_msg",      "Operación completada.\n\nEspacio recuperado: "           },
            { "mb_rep_done_title","Leedeo Cleaner"                                          },
            { "mb_rep_done_msg",  "Proceso de reparación completado."                       },
            { "mb_chkdsk_title",  "Error de Disco"                                          },
            { "mb_chkdsk_msg",    "Se han detectado errores en el disco. ¿Programar reparación al reiniciar?" },
            { "mb_chkdsk_sched",  "Reparación programada para el próximo reinicio."         },
            { "mb_chkdsk_ok",     "Listo"                                                   },
            { "mb_wg_notfound",   "No se encontró Winget en este PC."                       },
            { "mb_wg_error",      "Error al lanzar Winget: "                                },
            { "mb_start_error",   "Error al iniciar: "                                      },

            { "btn_registry",          "LIMPIEZA DE REGISTRO"                                                                              },
            { "hover_registry",        "🗂 LIMPIEZA DE REGISTRO\r\n\r\nBusca entradas huérfanas de desinstalaciones,\r\narrancadores automáticos rotos y referencias a archivos eliminados.\r\nMuestra la lista antes de borrar nada." },

            { "log_reg_start",         ">>> 🗂 LIMPIEZA DE REGISTRO — ANALIZANDO..."                                                      },
            { "log_reg_clean",         ">>> ✅ El registro está limpio. No se encontraron entradas huérfanas."                            },
            { "log_reg_found",         ">>> Se encontraron {0} entradas huérfanas:"                                                       },
            { "log_reg_confirm",       ">>> Revisa la lista anterior antes de confirmar."                                                 },
            { "log_reg_cancelled",     ">>> ℹ Operación cancelada. No se borró nada."                                                    },
            { "log_reg_deleting",      ">>> Eliminando entradas confirmadas..."                                                           },
            { "log_reg_done",          ">>> ✅ Listo. Eliminadas: {0}  |  Errores: {1}"                                                   },

            { "confirm_reg_title",     "Limpieza de Registro"                                                                             },
            { "confirm_reg_msg",       "Esta herramienta busca entradas que apuntan a archivos que ya no están en tu sistema.\n\nTe mostrará lo que encuentra antes de borrar nada.\n\n¿Iniciar el análisis?" },

            { "mb_reg_clean",          "El registro parece limpio. No se encontraron entradas huérfanas."                                 },
            { "mb_reg_confirm_title",  "Confirmar eliminación"                                                                            },
            { "mb_reg_confirm_msg",    "Se encontraron {0} entradas huérfanas.\n\n¿Eliminarlas todas?"                                    },
            { "mb_reg_done_msg",       "{0} entradas eliminadas correctamente."                                                           },
            { "mb_reg_done_partial",   "{0} entradas eliminadas.\n{1} no se pudieron eliminar (ver log para detalles)."                  },

            { "btn_save_log",     "💾 Guardar log"                                          },
            { "save_log_title",   "Guardar archivo de log"                                  },
            { "save_log_default", "leedeo_cleaner_log"                                      },
            { "save_log_success", "Log guardado correctamente."                             },
            { "save_log_error",   "No se pudo guardar el log: "                             },
            { "save_log_empty",   "El log está vacío, no hay nada que guardar."             },

            { "lbl_donate",       "Si esta herramienta te ha ahorrado tiempo, considera apoyar el proyecto:" },
            { "lbl_kofi",         "☕ Apoyar en Ko-fi (Leedeo)"                                              },
            { "lbl_pc",           "PC: "                                                                     },
            { "lbl_user",         "   |   USUARIO: "                                                         },
            { "lbl_system",       "\r\nSISTEMA: "                                                            },
            { "lbl_win_detected", "Windows Detectado"                                                        },
            { "lbl_ready",        "Leedeo Cleaner Ready"                                                     },
        };
    }

    // =========================================================================
    // PORTUGUESE
    // =========================================================================
    private static Dictionary<string, string> Portuguese()
    {
        return new Dictionary<string, string>
        {
            { "btn_quick",        "LIMPEZA RÁPIDA"     },
            { "btn_deep",         "LIMPEZA PROFUNDA"   },
            { "btn_repair",       "REPARAR SISTEMA"    },
            { "btn_update",       "ATUALIZAR APPS"     },

            { "hover_quick",      "► LIMPEZA RÁPIDA\r\n\r\nRemove temporários e limpa o cache DNS.\r\nPerfeito para manutenção diária."                                    },
            { "hover_deep",       "≡ LIMPEZA PROFUNDA\r\n\r\nElimina logs, atualizações antigas e lixo.\r\nRecomendado uma vez por mês."                                   },
            { "hover_repair",     "✚ REPARAÇÃO TOTAL\r\n\r\nAnalisa e repara Disco, Arquivos e Imagem.\r\nSe detectar erros, ajudará a corrigi-los."                       },
            { "hover_update",     "⚡ ATUALIZAR APPS (WINGET)\r\n\r\nBusca atualizações para seus programas.\r\nMostrará uma lista para confirmar antes de instalar."      },

            { "log_ready",        "Sistema pronto. Aguardando ordens..."  },
            { "log_space_before", "Espaço livre inicial: "                },
            { "log_separator",    "------------------------------------------------" },
            { "log_done",         ">>> ✅ PROCESSO CONCLUÍDO."            },
            { "log_freed",        ">>> ESPAÇO LIBERADO: "                 },
            { "log_error",        "❌ Erro inesperado: "                  },

            { "log_qc_start",     ">>> ► INICIANDO LIMPEZA RÁPIDA..."    },
            { "log_qc_user_tmp",  "> Temporários do usuário removidos."  },
            { "log_qc_sys_tmp",   "> Temporários do sistema removidos."  },
            { "log_qc_dns",       "> Cache DNS limpo."                   },
            { "log_qc_explorer",  "> Reiniciando Explorer..."            },

            { "log_dc_start",     ">>> ≡ INICIANDO LIMPEZA PROFUNDA..."           },
            { "log_dc_tmp",       "> Temporários removidos."                      },
            { "log_dc_wu",        "> Gerenciando Windows Update..."               },
            { "log_dc_wu_cache",  "> Cache de atualizações removido."             },
            { "log_dc_wu_start",  "> Serviço Windows Update reiniciado."          },
            { "log_dc_logs",      "> Removendo logs do sistema..."                },
            { "log_dc_defender",  "> Histórico do Defender removido."             },
            { "log_dc_prefetch",  "> Prefetch limpo."                             },

            { "log_rep_start",    ">>> ✚ REPARAÇÃO DO SISTEMA (ANÁLISE INTELIGENTE)" },
            { "log_rep_diag",     "Iniciando diagnóstico..."                          },
            { "log_rep_chk",      "[1/6] Verificando saúde do Disco (CHKDSK)..."     },
            { "log_rep_chk_ok",   "   ✅ DISCO: Saudável. Sem erros físicos."         },
            { "log_rep_chk_warn", "   ⚠️ DISCO: Anomalias detectadas."               },
            { "log_rep_chk_sched","   --> Reparação agendada para o próximo reinício." },
            { "log_rep_sfc",      "[2/6] Verificando integridade dos arquivos (SFC)..." },
            { "log_rep_sfc_ok",   "   ✅ ARQUIVOS: Integridade verificada."            },
            { "log_rep_sfc_fixed","   🛠️ ARQUIVOS: Erros detectados e reparados."     },
            { "log_rep_sfc_err",  "   ❌ ARQUIVOS: Erros complexos. Executando DISM..." },
            { "log_rep_scan",     "[3/6] DISM ScanHealth..."                           },
            { "log_rep_check",    "[4/6] DISM CheckHealth..."                          },
            { "log_rep_restore",  "[5/6] DISM RestoreHealth (pode demorar alguns minutos)..." },
            { "log_rep_rest_ok",  "   ✅ IMAGEM: Restauração concluída."               },
            { "log_rep_rest_warn","   ⚠️ IMAGEM: Houve um problema ao restaurar a imagem." },
            { "log_rep_cleanup",  "[6/6] Limpando componentes obsoletos (ComponentCleanup)..." },
            { "log_rep_done",     ">>> ✅ REPARAÇÃO CONCLUÍDA."                        },

            { "log_wg_start",     "\r\n>>> ⚡ INICIANDO GERENCIADOR DE ATUALIZAÇÕES...\r\n" },
            { "log_wg_search",    "Procurando Winget...\r\n"                                },
            { "log_wg_found",     "✅ Winget encontrado.\r\n"                               },
            { "log_wg_open",      "Abrindo lista de atualizações. Revise antes de aceitar.\r\n" },
            { "log_wg_notfound",  "❌ Erro: Winget não encontrado.\r\n"                     },
            { "log_wg_attention", "   ATENCAO: REVISE A LISTA ACIMA"                       },
            { "log_wg_warning",   "   Se NAO quiser atualizar algum programa, FECHE ESTA JANELA." },
            { "log_wg_confirm",   "   Para atualizar TUDO, pressione uma tecla..."                },
            { "log_wg_finished",  "PROCESSO CONCLUÍDO"                                           },

            { "confirm_qc_title", "Confirmar Limpeza Rápida"                                                                                    },
            { "confirm_qc_msg",   "Tem certeza que deseja iniciar a Limpeza Rápida?\n\nArquivos temporários e cache DNS serão removidos."        },
            { "confirm_dc_title", "Confirmar Limpeza Profunda"                                                                                   },
            { "confirm_dc_msg",   "Deseja iniciar a Limpeza Profunda?\n\nHistóricos, logs e cache de atualizações serão removidos."              },
            { "confirm_rep_title","Confirmar Reparação do Sistema"                                                                               },
            { "confirm_rep_msg",  "ATENÇÃO: Este processo pode demorar entre 20 e 30 minutos.\n\nDeseja iniciar o diagnóstico e reparação?"      },
            { "confirm_upd_title","Atualizar Aplicativos"                                                                                        },
            { "confirm_upd_msg",  "O Winget será aberto para buscar atualizações.\n\nUma lista será exibida para você confirmar antes de instalar.\nContinuar?" },

            { "mb_done_title",    "Leedeo Cleaner"                                          },
            { "mb_done_msg",      "Operação concluída.\n\nEspaço recuperado: "              },
            { "mb_rep_done_title","Leedeo Cleaner"                                          },
            { "mb_rep_done_msg",  "Processo de reparação concluído."                        },
            { "mb_chkdsk_title",  "Erro de Disco"                                           },
            { "mb_chkdsk_msg",    "Erros detectados no disco. Agendar reparação no próximo reinício?" },
            { "mb_chkdsk_sched",  "Reparação agendada para o próximo reinício."             },
            { "mb_chkdsk_ok",     "Pronto"                                                  },
            { "mb_wg_notfound",   "Winget não encontrado neste PC."                         },
            { "mb_wg_error",      "Erro ao iniciar Winget: "                                },
            { "mb_start_error",   "Erro ao iniciar: "                                       },

            { "btn_registry",          "LIMPEZA DE REGISTRO"                                                                               },
            { "hover_registry",        "🗂 LIMPEZA DE REGISTRO\r\n\r\nProcura entradas órfãs de desinstalações,\r\nentradas de inicialização automática quebradas e referências a arquivos removidos.\r\nMostra a lista antes de apagar qualquer coisa." },

            { "log_reg_start",         ">>> 🗂 LIMPEZA DE REGISTRO — ANALISANDO..."                                                       },
            { "log_reg_clean",         ">>> ✅ O registro está limpo. Nenhuma entrada órfã encontrada."                                   },
            { "log_reg_found",         ">>> {0} entradas órfãs encontradas:"                                                              },
            { "log_reg_confirm",       ">>> Revise a lista acima antes de confirmar."                                                     },
            { "log_reg_cancelled",     ">>> ℹ Operação cancelada. Nada foi apagado."                                                     },
            { "log_reg_deleting",      ">>> Apagando entradas confirmadas..."                                                             },
            { "log_reg_done",          ">>> ✅ Concluído. Apagadas: {0}  |  Erros: {1}"                                                   },

            { "confirm_reg_title",     "Limpeza de Registro"                                                                              },
            { "confirm_reg_msg",       "Esta ferramenta procura entradas que apontam para arquivos que não existem mais no seu sistema.\n\nMostrará o que encontrar antes de apagar qualquer coisa.\n\nIniciar a análise?" },

            { "mb_reg_clean",          "O registro parece limpo. Nenhuma entrada órfã foi encontrada."                                    },
            { "mb_reg_confirm_title",  "Confirmar exclusão"                                                                               },
            { "mb_reg_confirm_msg",    "{0} entradas órfãs foram encontradas.\n\nApagar todas?"                                           },
            { "mb_reg_done_msg",       "{0} entradas apagadas com sucesso."                                                               },
            { "mb_reg_done_partial",   "{0} entradas apagadas.\n{1} não puderam ser removidas (ver log para detalhes)."                  },

            { "btn_save_log",     "💾 Salvar log"                                           },
            { "save_log_title",   "Salvar arquivo de log"                                   },
            { "save_log_default", "leedeo_cleaner_log"                                      },
            { "save_log_success", "Log salvo com sucesso."                                  },
            { "save_log_error",   "Não foi possível salvar o log: "                         },
            { "save_log_empty",   "O log está vazio, nada para salvar."                     },

            { "lbl_donate",       "Se esta ferramenta poupou seu tempo, considere apoiar o projeto:" },
            { "lbl_kofi",         "☕ Apoiar no Ko-fi (Leedeo)"                                      },
            { "lbl_pc",           "PC: "                                                             },
            { "lbl_user",         "   |   USUÁRIO: "                                                 },
            { "lbl_system",       "\r\nSISTEMA: "                                                    },
            { "lbl_win_detected", "Windows Detectado"                                                },
            { "lbl_ready",        "Leedeo Cleaner Pronto"                                            },
        };
    }

    // =========================================================================
    // ITALIAN
    // =========================================================================
    private static Dictionary<string, string> Italian()
    {
        return new Dictionary<string, string>
        {
            { "btn_quick",        "PULIZIA RAPIDA"     },
            { "btn_deep",         "PULIZIA PROFONDA"   },
            { "btn_repair",       "RIPARA SISTEMA"     },
            { "btn_update",       "AGGIORNA APP"       },

            { "hover_quick",      "► PULIZIA RAPIDA\r\n\r\nRimuove i file temporanei e svuota la cache DNS.\r\nPerfetto per la manutenzione quotidiana."                    },
            { "hover_deep",       "≡ PULIZIA PROFONDA\r\n\r\nElimina log, aggiornamenti vecchi e spazzatura.\r\nConsigliato una volta al mese."                            },
            { "hover_repair",     "✚ RIPARAZIONE TOTALE\r\n\r\nAnalizza e ripara Disco, File e Immagine.\r\nSe rileva errori, ti aiuterà a correggerli."                   },
            { "hover_update",     "⚡ AGGIORNA APP (WINGET)\r\n\r\nCerca aggiornamenti per i tuoi programmi.\r\nMostra un elenco per confermare prima di installare."      },

            { "log_ready",        "Sistema pronto. In attesa di ordini..."   },
            { "log_space_before", "Spazio libero iniziale: "                 },
            { "log_separator",    "------------------------------------------------" },
            { "log_done",         ">>> ✅ PROCESSO COMPLETATO."              },
            { "log_freed",        ">>> SPAZIO LIBERATO: "                    },
            { "log_error",        "❌ Errore imprevisto: "                   },

            { "log_qc_start",     ">>> ► AVVIO PULIZIA RAPIDA..."           },
            { "log_qc_user_tmp",  "> File temporanei utente eliminati."     },
            { "log_qc_sys_tmp",   "> File temporanei di sistema eliminati." },
            { "log_qc_dns",       "> Cache DNS svuotata."                   },
            { "log_qc_explorer",  "> Riavvio di Explorer..."                },

            { "log_dc_start",     ">>> ≡ AVVIO PULIZIA PROFONDA..."                  },
            { "log_dc_tmp",       "> File temporanei eliminati."                     },
            { "log_dc_wu",        "> Gestione di Windows Update..."                  },
            { "log_dc_wu_cache",  "> Cache degli aggiornamenti eliminata."           },
            { "log_dc_wu_start",  "> Servizio Windows Update riavviato."             },
            { "log_dc_logs",      "> Eliminazione dei log di sistema..."             },
            { "log_dc_defender",  "> Cronologia di Defender eliminata."              },
            { "log_dc_prefetch",  "> Prefetch pulito."                               },

            { "log_rep_start",    ">>> ✚ RIPARAZIONE SISTEMA (ANALISI INTELLIGENTE)" },
            { "log_rep_diag",     "Avvio diagnostica..."                              },
            { "log_rep_chk",      "[1/6] Analisi salute disco (CHKDSK)..."           },
            { "log_rep_chk_ok",   "   ✅ DISCO: Integro. Nessun errore fisico."      },
            { "log_rep_chk_warn", "   ⚠️ DISCO: Anomalie rilevate."                  },
            { "log_rep_chk_sched","   --> Riparazione pianificata per il prossimo riavvio." },
            { "log_rep_sfc",      "[2/6] Verifica integrità file (SFC)..."           },
            { "log_rep_sfc_ok",   "   ✅ FILE: Integrità verificata."                },
            { "log_rep_sfc_fixed","   🛠️ FILE: Errori rilevati e reparati."          },
            { "log_rep_sfc_err",  "   ❌ FILE: Errori complessi. Esecuzione DISM..." },
            { "log_rep_scan",     "[3/6] DISM ScanHealth..."                         },
            { "log_rep_check",    "[4/6] DISM CheckHealth..."                        },
            { "log_rep_restore",  "[5/6] DISM RestoreHealth (potrebbe richiedere alcuni minuti)..." },
            { "log_rep_rest_ok",  "   ✅ IMMAGINE: Ripristino completato."           },
            { "log_rep_rest_warn","   ⚠️ IMMAGINE: Si è verificato un problema durante il ripristino." },
            { "log_rep_cleanup",  "[6/6] Pulizia componenti obsoleti (ComponentCleanup)..." },
            { "log_rep_done",     ">>> ✅ RIPARAZIONE COMPLETATA."                   },

            { "log_wg_start",     "\r\n>>> ⚡ AVVIO GESTORE AGGIORNAMENTI...\r\n"   },
            { "log_wg_search",    "Ricerca Winget...\r\n"                            },
            { "log_wg_found",     "✅ Winget trovato.\r\n"                            },
            { "log_wg_open",      "Apertura lista aggiornamenti. Controlla prima di accettare.\r\n" },
            { "log_wg_notfound",  "❌ Errore: Winget non trovato.\r\n"               },
            { "log_wg_attention", "   ATTENZIONE: CONTROLLA LA LISTA SOPRA"         },
            { "log_wg_warning",   "   Se NON vuoi aggiornare un programma, CHIUDI QUESTA FINESTRA." },
            { "log_wg_confirm",   "   Per aggiornare TUTTO, premi un tasto..."                      },
            { "log_wg_finished",  "PROCESSO COMPLETATO"                                             },

            { "confirm_qc_title", "Conferma Pulizia Rapida"                                                                               },
            { "confirm_qc_msg",   "Sei sicuro di voler avviare la Pulizia Rapida?\n\nI file temporanei e la cache DNS verranno rimossi."  },
            { "confirm_dc_title", "Conferma Pulizia Profonda"                                                                             },
            { "confirm_dc_msg",   "Vuoi avviare la Pulizia Profonda?\n\nVerranno rimossi log, cache aggiornamenti e cronologie."         },
            { "confirm_rep_title","Conferma Riparazione Sistema"                                                                          },
            { "confirm_rep_msg",  "ATTENZIONE: Questo processo può richiedere 20-30 minuti.\n\nAvviare la diagnostica e riparazione?"    },
            { "confirm_upd_title","Aggiorna Applicazioni"                                                                                 },
            { "confirm_upd_msg",  "Winget verrà aperto per cercare aggiornamenti.\n\nVerrà mostrato un elenco per confermare prima di installare.\nContinuare?" },

            { "mb_done_title",    "Leedeo Cleaner"                                          },
            { "mb_done_msg",      "Operazione completata.\n\nSpazio recuperato: "           },
            { "mb_rep_done_title","Leedeo Cleaner"                                          },
            { "mb_rep_done_msg",  "Processo di riparazione completato."                     },
            { "mb_chkdsk_title",  "Errore Disco"                                            },
            { "mb_chkdsk_msg",    "Rilevati errori sul disco. Pianificare la riparazione al prossimo riavvio?" },
            { "mb_chkdsk_sched",  "Riparazione pianificata per il prossimo riavvio."        },
            { "mb_chkdsk_ok",     "Fatto"                                                   },
            { "mb_wg_notfound",   "Winget non trovato su questo PC."                        },
            { "mb_wg_error",      "Errore nell'avvio di Winget: "                           },
            { "mb_start_error",   "Errore di avvio: "                                       },

            { "btn_registry",          "PULIZIA REGISTRO"                                                                                  },
            { "hover_registry",        "🗂 PULIZIA REGISTRO\r\n\r\nTrova voci orfane di disinstallazioni,\r\nvoci di avvio automatico non funzionanti e riferimenti a file eliminati.\r\nMostra l'elenco prima di eliminare qualsiasi cosa." },

            { "log_reg_start",         ">>> 🗂 PULIZIA REGISTRO — SCANSIONE IN CORSO..."                                                  },
            { "log_reg_clean",         ">>> ✅ Il registro è pulito. Nessuna voce orfana trovata."                                        },
            { "log_reg_found",         ">>> Trovate {0} voci orfane:"                                                                    },
            { "log_reg_confirm",       ">>> Controlla l'elenco sopra prima di confermare."                                               },
            { "log_reg_cancelled",     ">>> ℹ Operazione annullata. Non è stato eliminato nulla."                                        },
            { "log_reg_deleting",      ">>> Eliminazione delle voci confermate..."                                                        },
            { "log_reg_done",          ">>> ✅ Completato. Eliminate: {0}  |  Errori: {1}"                                               },

            { "confirm_reg_title",     "Pulizia Registro"                                                                                 },
            { "confirm_reg_msg",       "Questo strumento cerca voci che puntano a file non più presenti nel sistema.\n\nMostrerà ciò che trova prima di eliminare qualsiasi cosa.\n\nAvviare la scansione?" },

            { "mb_reg_clean",          "Il registro sembra pulito. Nessuna voce orfana trovata."                                         },
            { "mb_reg_confirm_title",  "Conferma eliminazione"                                                                            },
            { "mb_reg_confirm_msg",    "Sono state trovate {0} voci orfane.\n\nEliminarle tutte?"                                        },
            { "mb_reg_done_msg",       "{0} voci eliminate con successo."                                                                 },
            { "mb_reg_done_partial",   "{0} voci eliminate.\n{1} non hanno potuto essere rimosse (vedere il log per i dettagli)."        },

            { "btn_save_log",     "💾 Salva log"                                            },
            { "save_log_title",   "Salva file di log"                                       },
            { "save_log_default", "leedeo_cleaner_log"                                      },
            { "save_log_success", "Log salvato con successo."                               },
            { "save_log_error",   "Impossibile salvare il log: "                            },
            { "save_log_empty",   "Il log è vuoto, niente da salvare."                      },

            { "lbl_donate",       "Se questo strumento ti ha fatto risparmiare tempo, considera di supportare il progetto:" },
            { "lbl_kofi",         "☕ Supporta su Ko-fi (Leedeo)"                                                           },
            { "lbl_pc",           "PC: "                                                                                    },
            { "lbl_user",         "   |   UTENTE: "                                                                         },
            { "lbl_system",       "\r\nSISTEMA: "                                                                           },
            { "lbl_win_detected", "Windows Rilevato"                                                                        },
            { "lbl_ready",        "Leedeo Cleaner Pronto"                                                                   },
        };
    }

    // =========================================================================
    // FRENCH
    // =========================================================================
    private static Dictionary<string, string> French()
    {
        return new Dictionary<string, string>
        {
            { "btn_quick",        "NETTOYAGE RAPIDE"   },
            { "btn_deep",         "NETTOYAGE PROFOND"  },
            { "btn_repair",       "RÉPARER SYSTÈME"    },
            { "btn_update",       "METTRE À JOUR"      },

            { "hover_quick",      "► NETTOYAGE RAPIDE\r\n\r\nSupprime les fichiers temporaires et vide le cache DNS.\r\nParfait pour la maintenance quotidienne."              },
            { "hover_deep",       "≡ NETTOYAGE PROFOND\r\n\r\nSupprime les journaux, mises à jour anciennes et les déchets.\r\nRecommandé une fois par mois."                 },
            { "hover_repair",     "✚ RÉPARATION TOTALE\r\n\r\nAnalyse et répare le Disque, les Fichiers et l'Image.\r\nSi des erreurs sont détectées, il vous aidera à les corriger." },
            { "hover_update",     "⚡ METTRE À JOUR (WINGET)\r\n\r\nRecherche des mises à jour pour vos programmes.\r\nAffiche une liste pour confirmer avant d'installer."   },

            { "log_ready",        "Système prêt. En attente d'ordres..."  },
            { "log_space_before", "Espace libre initial : "               },
            { "log_separator",    "------------------------------------------------" },
            { "log_done",         ">>> ✅ PROCESSUS TERMINÉ."             },
            { "log_freed",        ">>> ESPACE LIBÉRÉ : "                  },
            { "log_error",        "❌ Erreur inattendue : "               },

            { "log_qc_start",     ">>> ► DÉMARRAGE DU NETTOYAGE RAPIDE..."       },
            { "log_qc_user_tmp",  "> Fichiers temporaires utilisateur supprimés." },
            { "log_qc_sys_tmp",   "> Fichiers temporaires système supprimés."     },
            { "log_qc_dns",       "> Cache DNS vidé."                            },
            { "log_qc_explorer",  "> Redémarrage de l'Explorateur..."           },

            { "log_dc_start",     ">>> ≡ DÉMARRAGE DU NETTOYAGE PROFOND..."          },
            { "log_dc_tmp",       "> Fichiers temporaires supprimés."                },
            { "log_dc_wu",        "> Gestion de Windows Update..."                   },
            { "log_dc_wu_cache",  "> Cache des mises à jour supprimé."              },
            { "log_dc_wu_start",  "> Service Windows Update redémarré."             },
            { "log_dc_logs",      "> Suppression des journaux système..."            },
            { "log_dc_defender",  "> Historique Defender supprimé."                 },
            { "log_dc_prefetch",  "> Prefetch nettoyé."                             },

            { "log_rep_start",    ">>> ✚ RÉPARATION SYSTÈME (ANALYSE INTELLIGENTE)" },
            { "log_rep_diag",     "Démarrage du diagnostic..."                       },
            { "log_rep_chk",      "[1/6] Analyse de l'état du disque (CHKDSK)..."   },
            { "log_rep_chk_ok",   "   ✅ DISQUE : Sain. Aucune erreur physique."    },
            { "log_rep_chk_warn", "   ⚠️ DISQUE : Anomalies détectées."             },
            { "log_rep_chk_sched","   --> Réparation planifiée pour le prochain redémarrage." },
            { "log_rep_sfc",      "[2/6] Vérification de l'intégrité des fichiers (SFC)..." },
            { "log_rep_sfc_ok",   "   ✅ FICHIERS : Intégrité vérifiée."            },
            { "log_rep_sfc_fixed","   🛠️ FICHIERS : Erreurs détectées et réparées." },
            { "log_rep_sfc_err",  "   ❌ FICHIERS : Erreurs complexes. Exécution de DISM..." },
            { "log_rep_scan",     "[3/6] DISM ScanHealth..."                         },
            { "log_rep_check",    "[4/6] DISM CheckHealth..."                        },
            { "log_rep_restore",  "[5/6] DISM RestoreHealth (peut prendre plusieurs minutes)..." },
            { "log_rep_rest_ok",  "   ✅ IMAGE : Restauration terminée."             },
            { "log_rep_rest_warn","   ⚠️ IMAGE : Un problème est survenu lors de la restauration." },
            { "log_rep_cleanup",  "[6/6] Nettoyage des composants obsolètes (ComponentCleanup)..." },
            { "log_rep_done",     ">>> ✅ RÉPARATION TERMINÉE."                      },

            { "log_wg_start",     "\r\n>>> ⚡ DÉMARRAGE DU GESTIONNAIRE DE MISES À JOUR...\r\n" },
            { "log_wg_search",    "Recherche de Winget...\r\n"                                   },
            { "log_wg_found",     "✅ Winget trouvé.\r\n"                                         },
            { "log_wg_open",      "Ouverture de la liste des mises à jour. Vérifiez avant d'accepter.\r\n" },
            { "log_wg_notfound",  "❌ Erreur : Winget introuvable.\r\n"                           },
            { "log_wg_attention", "   ATTENTION : VÉRIFIEZ LA LISTE CI-DESSUS"                   },
            { "log_wg_warning",   "   Si vous NE voulez PAS mettre à jour un programme, FERMEZ CETTE FENÊTRE." },
            { "log_wg_confirm",   "   Pour tout mettre à jour, appuyez sur une touche..."                      },
            { "log_wg_finished",  "PROCESSUS TERMINÉ"                                                         },

            { "confirm_qc_title", "Confirmer le Nettoyage Rapide"                                                                               },
            { "confirm_qc_msg",   "Êtes-vous sûr de vouloir démarrer le Nettoyage Rapide?\n\nLes fichiers temporaires et le cache DNS seront supprimés." },
            { "confirm_dc_title", "Confirmer le Nettoyage Profond"                                                                              },
            { "confirm_dc_msg",   "Voulez-vous démarrer le Nettoyage Profond?\n\nLes journaux, le cache des mises à jour et les historiques seront supprimés." },
            { "confirm_rep_title","Confirmer la Réparation Système"                                                                             },
            { "confirm_rep_msg",  "ATTENTION : Ce processus peut prendre 20 à 30 minutes.\n\nDémarrer le diagnostic et la réparation?"          },
            { "confirm_upd_title","Mettre à Jour les Applications"                                                                              },
            { "confirm_upd_msg",  "Winget sera ouvert pour rechercher des mises à jour.\n\nUne liste sera affichée pour confirmer avant d'installer.\nContinuer?" },

            { "mb_done_title",    "Leedeo Cleaner"                                              },
            { "mb_done_msg",      "Opération terminée.\n\nEspace récupéré : "                   },
            { "mb_rep_done_title","Leedeo Cleaner"                                              },
            { "mb_rep_done_msg",  "Processus de réparation terminé."                            },
            { "mb_chkdsk_title",  "Erreur Disque"                                               },
            { "mb_chkdsk_msg",    "Des erreurs ont été détectées sur le disque. Planifier la réparation au prochain redémarrage?" },
            { "mb_chkdsk_sched",  "Réparation planifiée pour le prochain redémarrage."          },
            { "mb_chkdsk_ok",     "OK"                                                          },
            { "mb_wg_notfound",   "Winget introuvable sur ce PC."                               },
            { "mb_wg_error",      "Erreur lors du lancement de Winget : "                       },
            { "mb_start_error",   "Erreur au démarrage : "                                      },

            { "btn_registry",          "NETTOYAGE REGISTRE"                                                                                },
            { "hover_registry",        "🗂 NETTOYAGE REGISTRE\r\n\r\nRecherche les entrées orphelines de désinstallations,\r\nles entrées de démarrage automatique cassées et les références à des fichiers supprimés.\r\nAffiche la liste avant de supprimer quoi que ce soit." },

            { "log_reg_start",         ">>> 🗂 NETTOYAGE REGISTRE — ANALYSE EN COURS..."                                                 },
            { "log_reg_clean",         ">>> ✅ Le registre est propre. Aucune entrée orpheline trouvée."                                 },
            { "log_reg_found",         ">>> {0} entrées orphelines trouvées :"                                                           },
            { "log_reg_confirm",       ">>> Vérifiez la liste ci-dessus avant de confirmer."                                             },
            { "log_reg_cancelled",     ">>> ℹ Opération annulée. Rien n'a été supprimé."                                                },
            { "log_reg_deleting",      ">>> Suppression des entrées confirmées..."                                                        },
            { "log_reg_done",          ">>> ✅ Terminé. Supprimées : {0}  |  Erreurs : {1}"                                              },

            { "confirm_reg_title",     "Nettoyage Registre"                                                                               },
            { "confirm_reg_msg",       "Cet outil recherche des entrées qui pointent vers des fichiers qui n'existent plus sur votre système.\n\nIl vous montrera ce qu'il trouve avant de supprimer quoi que ce soit.\n\nLancer l'analyse ?" },

            { "mb_reg_clean",          "Le registre semble propre. Aucune entrée orpheline n'a été trouvée."                             },
            { "mb_reg_confirm_title",  "Confirmer la suppression"                                                                         },
            { "mb_reg_confirm_msg",    "{0} entrées orphelines ont été trouvées.\n\nToutes les supprimer ?"                              },
            { "mb_reg_done_msg",       "{0} entrées supprimées avec succès."                                                              },
            { "mb_reg_done_partial",   "{0} entrées supprimées.\n{1} n'ont pas pu être supprimées (voir le log pour les détails)."       },

            { "btn_save_log",     "💾 Enregistrer le log"                                       },
            { "save_log_title",   "Enregistrer le fichier de log"                               },
            { "save_log_default", "leedeo_cleaner_log"                                          },
            { "save_log_success", "Log enregistré avec succès."                                 },
            { "save_log_error",   "Impossible d'enregistrer le log : "                          },
            { "save_log_empty",   "Le log est vide, rien à enregistrer."                        },

            { "lbl_donate",       "Si cet outil vous a fait gagner du temps, envisagez de soutenir le projet :" },
            { "lbl_kofi",         "☕ Soutenir sur Ko-fi (Leedeo)"                                              },
            { "lbl_pc",           "PC : "                                                                       },
            { "lbl_user",         "   |   UTILISATEUR : "                                                       },
            { "lbl_system",       "\r\nSYSTÈME : "                                                              },
            { "lbl_win_detected", "Windows Détecté"                                                             },
            { "lbl_ready",        "Leedeo Cleaner Prêt"                                                         },
        };
    }

    // =========================================================================
    // GERMAN
    // =========================================================================
    private static Dictionary<string, string> German()
    {
        return new Dictionary<string, string>
        {
            { "btn_quick",        "SCHNELLREINIGUNG"   },
            { "btn_deep",         "TIEFENREINIGUNG"    },
            { "btn_repair",       "SYSTEM REPARIEREN"  },
            { "btn_update",       "APPS AKTUALISIEREN" },

            { "hover_quick",      "► SCHNELLREINIGUNG\r\n\r\nEntfernt temporäre Dateien und leert den DNS-Cache.\r\nPerfekt für die tägliche Wartung."                          },
            { "hover_deep",       "≡ TIEFENREINIGUNG\r\n\r\nEntfernt Protokolle, alte Updates und Datenmüll.\r\nEmpfohlen einmal im Monat."                                    },
            { "hover_repair",     "✚ VOLLSTÄNDIGE REPARATUR\r\n\r\nAnalysiert und repariert Festplatte, Dateien und Image.\r\nBei Fehlern wird Ihnen geholfen, diese zu beheben." },
            { "hover_update",     "⚡ APPS AKTUALISIEREN (WINGET)\r\n\r\nSucht nach Updates für Ihre Programme.\r\nZeigt eine Liste zur Bestätigung vor der Installation."     },

            { "log_ready",        "System bereit. Warte auf Befehle..."   },
            { "log_space_before", "Freier Speicher vorher: "              },
            { "log_separator",    "------------------------------------------------" },
            { "log_done",         ">>> ✅ VORGANG ABGESCHLOSSEN."         },
            { "log_freed",        ">>> FREIGEGEBENER SPEICHER: "          },
            { "log_error",        "❌ Unerwarteter Fehler: "              },

            { "log_qc_start",     ">>> ► SCHNELLREINIGUNG WIRD GESTARTET..."     },
            { "log_qc_user_tmp",  "> Temporäre Benutzerdateien gelöscht."        },
            { "log_qc_sys_tmp",   "> Temporäre Systemdateien gelöscht."          },
            { "log_qc_dns",       "> DNS-Cache geleert."                         },
            { "log_qc_explorer",  "> Explorer wird neu gestartet..."             },

            { "log_dc_start",     ">>> ≡ TIEFENREINIGUNG WIRD GESTARTET..."             },
            { "log_dc_tmp",       "> Temporäre Dateien gelöscht."                       },
            { "log_dc_wu",        "> Windows Update wird verwaltet..."                  },
            { "log_dc_wu_cache",  "> Update-Cache gelöscht."                            },
            { "log_dc_wu_start",  "> Windows Update-Dienst neu gestartet."              },
            { "log_dc_logs",      "> Systemprotokolle werden gelöscht..."               },
            { "log_dc_defender",  "> Defender-Verlauf gelöscht."                        },
            { "log_dc_prefetch",  "> Prefetch bereinigt."                               },

            { "log_rep_start",    ">>> ✚ SYSTEMREPARATUR (INTELLIGENTE ANALYSE)"        },
            { "log_rep_diag",     "Diagnose wird gestartet..."                           },
            { "log_rep_chk",      "[1/6] Festplattenzustand prüfen (CHKDSK)..."         },
            { "log_rep_chk_ok",   "   ✅ FESTPLATTE: Gesund. Keine physischen Fehler."  },
            { "log_rep_chk_warn", "   ⚠️ FESTPLATTE: Anomalien erkannt."                },
            { "log_rep_chk_sched","   --> Reparatur für den nächsten Neustart geplant." },
            { "log_rep_sfc",      "[2/6] Dateiintegrität prüfen (SFC)..."               },
            { "log_rep_sfc_ok",   "   ✅ DATEIEN: Integrität bestätigt."                },
            { "log_rep_sfc_fixed","   🛠️ DATEIEN: Fehler erkannt und repariert."        },
            { "log_rep_sfc_err",  "   ❌ DATEIEN: Komplexe Fehler. DISM wird ausgeführt..." },
            { "log_rep_scan",     "[3/6] DISM ScanHealth..."                             },
            { "log_rep_check",    "[4/6] DISM CheckHealth..."                            },
            { "log_rep_restore",  "[5/6] DISM RestoreHealth (kann einige Minuten dauern)..." },
            { "log_rep_rest_ok",  "   ✅ IMAGE: Wiederherstellung abgeschlossen."        },
            { "log_rep_rest_warn","   ⚠️ IMAGE: Beim Wiederherstellen ist ein Fehler aufgetreten." },
            { "log_rep_cleanup",  "[6/6] Veraltete Komponenten bereinigen (ComponentCleanup)..." },
            { "log_rep_done",     ">>> ✅ REPARATUR ABGESCHLOSSEN."                      },

            { "log_wg_start",     "\r\n>>> ⚡ UPDATE-MANAGER WIRD GESTARTET...\r\n"    },
            { "log_wg_search",    "Winget wird gesucht...\r\n"                         },
            { "log_wg_found",     "✅ Winget gefunden.\r\n"                             },
            { "log_wg_open",      "Update-Liste wird geöffnet. Bitte vor dem Akzeptieren prüfen.\r\n" },
            { "log_wg_notfound",  "❌ Fehler: Winget nicht gefunden.\r\n"              },
            { "log_wg_attention", "   ACHTUNG: LISTE OBEN PRÜFEN"                     },
            { "log_wg_warning",   "   Wenn Sie ein Programm NICHT aktualisieren möchten, SCHLIESSEN SIE DIESES FENSTER." },
            { "log_wg_confirm",   "   Um ALLES zu aktualisieren, drücken Sie eine Taste..."                              },
            { "log_wg_finished",  "VORGANG ABGESCHLOSSEN"                                                                },

            { "confirm_qc_title", "Schnellreinigung bestätigen"                                                                               },
            { "confirm_qc_msg",   "Möchten Sie die Schnellreinigung wirklich starten?\n\nTemporäre Dateien und DNS-Cache werden gelöscht."    },
            { "confirm_dc_title", "Tiefenreinigung bestätigen"                                                                                },
            { "confirm_dc_msg",   "Möchten Sie die Tiefenreinigung starten?\n\nProtokolle, Update-Cache und Verläufe werden gelöscht."        },
            { "confirm_rep_title","Systemreparatur bestätigen"                                                                                },
            { "confirm_rep_msg",  "ACHTUNG: Dieser Vorgang kann 20–30 Minuten dauern.\n\nDiagnose und Reparatur starten?"                    },
            { "confirm_upd_title","Apps aktualisieren"                                                                                        },
            { "confirm_upd_msg",  "Winget wird geöffnet, um nach Updates zu suchen.\n\nEine Liste wird angezeigt, damit Sie vor der Installation bestätigen können.\nFortfahren?" },

            { "mb_done_title",    "Leedeo Cleaner"                                              },
            { "mb_done_msg",      "Vorgang abgeschlossen.\n\nFreigegebener Speicher: "          },
            { "mb_rep_done_title","Leedeo Cleaner"                                              },
            { "mb_rep_done_msg",  "Reparaturvorgang abgeschlossen."                             },
            { "mb_chkdsk_title",  "Festplattenfehler"                                           },
            { "mb_chkdsk_msg",    "Fehler auf der Festplatte erkannt. Reparatur beim nächsten Neustart planen?" },
            { "mb_chkdsk_sched",  "Reparatur für den nächsten Neustart geplant."                },
            { "mb_chkdsk_ok",     "OK"                                                          },
            { "mb_wg_notfound",   "Winget wurde auf diesem PC nicht gefunden."                  },
            { "mb_wg_error",      "Fehler beim Starten von Winget: "                            },
            { "mb_start_error",   "Startfehler: "                                               },

            { "btn_registry",          "REGISTRIERUNG BEREINIGEN"                                                                          },
            { "hover_registry",        "🗂 REGISTRIERUNG BEREINIGEN\r\n\r\nSucht nach verwaisten Deinstallationseinträgen,\r\ndefekten Autostart-Einträgen und Verweisen auf gelöschte Dateien.\r\nZeigt die Liste an, bevor etwas gelöscht wird." },

            { "log_reg_start",         ">>> 🗂 REGISTRIERUNG BEREINIGEN — SUCHE LÄUFT..."                                                },
            { "log_reg_clean",         ">>> ✅ Die Registrierung ist sauber. Keine verwaisten Einträge gefunden."                        },
            { "log_reg_found",         ">>> {0} verwaiste Einträge gefunden:"                                                            },
            { "log_reg_confirm",       ">>> Überprüfen Sie die Liste oben, bevor Sie bestätigen."                                        },
            { "log_reg_cancelled",     ">>> ℹ Vorgang abgebrochen. Es wurde nichts gelöscht."                                           },
            { "log_reg_deleting",      ">>> Bestätigte Einträge werden gelöscht..."                                                       },
            { "log_reg_done",          ">>> ✅ Fertig. Gelöscht: {0}  |  Fehler: {1}"                                                    },

            { "confirm_reg_title",     "Registrierung bereinigen"                                                                         },
            { "confirm_reg_msg",       "Dieses Tool sucht nach Einträgen, die auf Dateien verweisen, die nicht mehr auf Ihrem System vorhanden sind.\n\nEs zeigt Ihnen, was gefunden wurde, bevor etwas gelöscht wird.\n\nScan starten?" },

            { "mb_reg_clean",          "Die Registrierung scheint sauber zu sein. Keine verwaisten Einträge gefunden."                   },
            { "mb_reg_confirm_title",  "Löschen bestätigen"                                                                               },
            { "mb_reg_confirm_msg",    "{0} verwaiste Einträge wurden gefunden.\n\nAlle löschen?"                                        },
            { "mb_reg_done_msg",       "{0} Einträge erfolgreich gelöscht."                                                               },
            { "mb_reg_done_partial",   "{0} Einträge gelöscht.\n{1} konnten nicht entfernt werden (Details im Log)."                     },

            { "btn_save_log",     "💾 Log speichern"                                            },
            { "save_log_title",   "Log-Datei speichern"                                         },
            { "save_log_default", "leedeo_cleaner_log"                                          },
            { "save_log_success", "Log erfolgreich gespeichert."                                },
            { "save_log_error",   "Log konnte nicht gespeichert werden: "                       },
            { "save_log_empty",   "Das Log ist leer, nichts zu speichern."                      },

            { "lbl_donate",       "Wenn dieses Tool Ihnen Zeit gespart hat, erwägen Sie, das Projekt zu unterstützen:" },
            { "lbl_kofi",         "☕ Auf Ko-fi unterstützen (Leedeo)"                                                 },
            { "lbl_pc",           "PC: "                                                                               },
            { "lbl_user",         "   |   BENUTZER: "                                                                  },
            { "lbl_system",       "\r\nSYSTEM: "                                                                       },
            { "lbl_win_detected", "Windows erkannt"                                                                    },
            { "lbl_ready",        "Leedeo Cleaner Bereit"                                                              },
        };
    }
}
